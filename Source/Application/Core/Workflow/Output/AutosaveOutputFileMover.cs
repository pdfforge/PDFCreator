using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow.Output
{
    public class AutosaveOutputFileMover : OutputFileMoverBase
    {
        private readonly IPdfProcessor _pdfProcessor;

        public AutosaveOutputFileMover(IUniqueFilenameFactory uniqueFilenameFactory, IFile file, IPathUtil pathUtil, IDirectoryHelper directoryHelper, IPdfProcessor pdfProcessor)
        {
            _pdfProcessor = pdfProcessor;
            UniqueFilenameFactory = uniqueFilenameFactory;
            File = file;
            PathUtil = pathUtil;
            DirectoryHelper = directoryHelper;
        }

        protected override IUniqueFilenameFactory UniqueFilenameFactory { get; }
        protected override IDirectoryHelper DirectoryHelper { get; }
        protected override IFile File { get; }
        protected override IPathUtil PathUtil { get; }

        protected override Task<QueryResult<string>> HandleInvalidRootedPath(string filename, OutputFormat outputFormat)
        {
            return Task.FromResult(new QueryResult<string>(false, null));
        }

        protected override Task<QueryResult<string>> HandleFirstFileFailed(string filename, OutputFormat outputFormat)
        {
            return Task.FromResult(new QueryResult<string>(false, null));
        }

        protected override HandleCopyErrorResult QueryHandleCopyError(int fileNumber)
        {
            return HandleCopyErrorResult.EnsureUniqueFilename;
        }

        protected override bool ShouldApplyUniqueFilename(Job job)
        {
            if (job.Profile.AutoSave.AutoMergeFiles && !job.Profile.OutputFormat.IsPdf())
                return true;

            return job.Profile.AutoSave.EnsureUniqueFilenames;
        }

        protected override bool ShouldApplyMerger(Job job)
        {
            if (!job.Profile.OutputFormat.IsPdf())
                return false;

            return job.Profile.AutoSave.AutoMergeFiles;
        }

        protected override bool AppendFile(string tempFile, string outputFile)
        {
            try
            {
                _pdfProcessor.MergePDFs(outputFile, tempFile);
                Logger.Debug("Append output file \"{0}\" \r\ninto \"{1}\"", tempFile, outputFile);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn("Error while append into target file.\r\nfrom\"{0}\" \r\nto \"{1}\"\r\n{2}", tempFile, outputFile, ex.Message);
            }
            return false;
        }
    }
}
