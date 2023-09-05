using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Utilities;
using System;
using System.IO;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath
{
    public interface ITargetFilePathComposer
    {
        string ComposeTargetFilePath(Job job);
    }

    public abstract class TargetFilePathComposerBase : ITargetFilePathComposer
    {
        private OutputFormatHelper OutputFormatHelper { get; }
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPathUtil _pathUtil;
        private readonly ISplitDocumentFilePathHelper _splitDocumentFilePathHelper;
        private readonly ITempFolderProvider _tempFolderProvider;
        private readonly OutputFormatHelper _outputFormatHelper;

        protected TargetFilePathComposerBase(IPathUtil pathUtil, ISplitDocumentFilePathHelper splitDocumentFilePathHelper, 
            OutputFormatHelper outputFormatHelper, ITempFolderProvider tempFolderProvider)
        {
            OutputFormatHelper = outputFormatHelper;
            _pathUtil = pathUtil;
            _splitDocumentFilePathHelper = splitDocumentFilePathHelper;
            _tempFolderProvider = tempFolderProvider;
            _outputFormatHelper = new OutputFormatHelper();
        }

        public string ComposeTargetFilePath(Job job)
        {
            //Consider OutputFileParameter
            if (!string.IsNullOrWhiteSpace(job.JobInfo.OutputFileParameter))
                if (_pathUtil.IsValidRootedPath(job.JobInfo.OutputFileParameter))
                    return job.JobInfo.OutputFileParameter;

            var outputFolder = DetermineOutputFolder(job);
            var outputFileName = ComposeOutputFileName(job);
            var filePath = PathSafe.Combine(outputFolder, outputFileName);

            //Keep long filename for interactive
            if (!job.Profile.AutoSave.Enabled)
                return filePath;

            try
            {
                filePath = _pathUtil.CheckAndShortenTooLongPath(filePath);
            }
            catch (ArgumentException)
            {
                throw new WorkflowException("Filepath is only a directory or the directory itself is already too long to append a useful filename under the limits of Windows (max " + _pathUtil.MAX_PATH + " characters):\n"
                                            + filePath);
            }

            return filePath;
        }

        private string DetermineOutputFolder(Job job)
        {
            if (job.Profile.SaveFileTemporary)
            {
                var tempFolder = PathSafe.Combine(_tempFolderProvider.TempFolder,
                        "Job_tempsave_" + PathSafe.GetFileNameWithoutExtension(Path.GetRandomFileName()));
                return tempFolder;
            }

            var outputFolder = ValidName.MakeValidFolderName(job.TokenReplacer.ReplaceTokens(job.Profile.TargetDirectory));

            if (!job.Profile.AutoSave.Enabled)
            {
                outputFolder = ConsiderLastSaveDirectory(outputFolder, job);
                // MyDocuments folder as fallback for interactive
                if (string.IsNullOrWhiteSpace(outputFolder))
                    outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            return outputFolder;
        }

        protected abstract string ConsiderLastSaveDirectory(string outputFolder, Job job);

        private string ComposeOutputFileName(Job job)
        {
            var outputFileNameWithReplacedTokens = job.TokenReplacer.ReplaceTokens(job.Profile.FileNameTemplate);
            var outputFileName = ValidName.MakeValidFileName(outputFileNameWithReplacedTokens);

            outputFileName = OutputFormatHelper.RemoveKnownFileExtension(outputFileName);

            if (string.IsNullOrEmpty(outputFileName))
            {
                if (job.Profile.AutoSave.Enabled)
                {
                    outputFileName = "_";
                    _logger.Warn("Filename is empty and will be set to \'_\'");
                }
            }

            outputFileName += _outputFormatHelper.GetExtension(job.Profile.OutputFormat);

            if (job.JobInfo.SourceFiles.First().IsSplitJob
                && job.Profile.AutoSave.Enabled && job.Profile.AutoSave.ExistingFileBehaviour == AutoSaveExistingFileBehaviour.Overwrite)
            {
                var splitJobParentFileName = PathSafe.GetFileName(job.JobInfo.SourceFiles.First().SplitJobParentFilePath);
                if (outputFileName.Equals(splitJobParentFileName))
                    outputFileName = _splitDocumentFilePathHelper.GetSplitDocumentFilePath(outputFileName);
            }

            return outputFileName;
        }
    }
}
