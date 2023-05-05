using NLog;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IMergeBeforeActionsHelper
    {
        void HandleMerge(Job job);
    }

    public class MergeBeforeActionsHelper : IMergeBeforeActionsHelper

    {
        private readonly IFile _file;
        private readonly IPath _path;
        private readonly ISourceFileInfoDuplicator _sourceFileInfoDuplicator;
        private readonly IPdfProcessor _pdfProcessor;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public MergeBeforeActionsHelper(IFile file, IPath path,
            ISourceFileInfoDuplicator sourceFileInfoDuplicator, IPdfProcessor pdfProcessor)
        {
            _file = file;
            _path = path;
            _sourceFileInfoDuplicator = sourceFileInfoDuplicator;
            _pdfProcessor = pdfProcessor;
        }

        public void HandleMerge(Job job)
        {
            if (!job.Profile.AutoSave.Enabled)
                return;

            if (job.Profile.AutoSave.ExistingFileBehaviour != AutoSaveExistingFileBehaviour.MergeBeforeModifyActions)
                return;

            if (!_file.Exists(job.OutputFileTemplate))
                return;

            if (!job.Profile.OutputFormat.IsPdf())
            {
                _logger.Warn("The output format is not a PDF file. Automatic merging is not possible for non-PDF files.");
                return;
            }

            PrependFileToSourceFiles(job.OutputFileTemplate, job);

            _logger.Debug($"File with path {job.OutputFileTemplate} has been found. " +
                          "An auto-merge will be executed before the modify and send actions.");
        }

        private void PrependFileToSourceFiles(string existingFilePath, Job job)
        {
            var totalPages = GetTotalPagesOfFile(existingFilePath, job.Passwords.PdfOwnerPassword);
            if (totalPages != 0)
            {
                var destFilePath = _path.Combine(job.JobTempFolder, _path.GetFileName(existingFilePath));
                var sfi = _sourceFileInfoDuplicator.DuplicateProperties(job.JobInfo.SourceFiles.First(), job.Profile.Guid);
                sfi.Filename = destFilePath;
                sfi.TotalPages = totalPages;
                sfi.DocumentTitle = _path.GetFileNameWithoutExtension(existingFilePath);
                sfi.OriginalFilePath = existingFilePath;
                _file.Copy(existingFilePath, destFilePath);
                job.JobInfo.SourceFiles.Insert(0, sfi);

                // job.ExistingFileBehavior is set to Overwrite since we are already using the file in the job
                job.ExistingFileBehavior = ExistingFileBehaviour.Overwrite;
                return;
            }

            job.Profile.AutoSave.ExistingFileBehaviour = AutoSaveExistingFileBehaviour.EnsureUniqueFilenames;
        }

        private int GetTotalPagesOfFile(string file, string password)
        {
            try
            {
                return string.IsNullOrEmpty(password) ?
                    _pdfProcessor.GetNumberOfPages(file) :
                    _pdfProcessor.GetNumberOfPages(file, password);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "The file could not be merged.");
                throw;
            }
        }
    }
}
