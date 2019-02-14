using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Utilities;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow.ComposeTargetFilePath
{
    public interface ITargetFilePathComposer
    {
        string ComposeTargetFilePath(Job job);
    }

    public abstract class TargetFilePathComposerBase : ITargetFilePathComposer
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPathUtil _pathUtil;
        private readonly OutputFormatHelper _outputFormatHelper;

        protected TargetFilePathComposerBase(IPathUtil pathUtil)
        {
            _pathUtil = pathUtil;
            _outputFormatHelper = new OutputFormatHelper();
        }

        public string ComposeTargetFilePath(Job job)
        {
            //Consider OutputFileParameter
            if (!string.IsNullOrWhiteSpace(job.JobInfo.OutputFileParameter))
                if (_pathUtil.IsValidRootedPath(job.JobInfo.OutputFileParameter))
                    return job.JobInfo.OutputFileParameter;

            var outputFolder = ComposeOutputFolder(job);
            var outputFileName = ComposeOutputFilename(job);
            var filePath = PathSafe.Combine(outputFolder, outputFileName);

            //Keep long filename for interactive
            if (!job.Profile.AutoSave.Enabled)
                return filePath;

            try
            {
                filePath = _pathUtil.EllipsisForTooLongPath(filePath);
                _logger.Debug("FilePath after ellipsis: " + filePath);
            }
            catch (ArgumentException)
            {
                throw new WorkflowException("Filepath is only a directory or the directory itself is already too long to append a useful filename under the limits of Windows (max " + _pathUtil.MAX_PATH + " characters):\n"
                                            + filePath);
            }

            return filePath;
        }

        private string ComposeOutputFolder(Job job)
        {
            var outputFolder = ValidName.MakeValidFolderName(job.TokenReplacer.ReplaceTokens(job.Profile.TargetDirectory));

            //Consider LastSaveDirectory
            if (!job.Profile.AutoSave.Enabled)
                outputFolder = ConsiderLastSaveDirectory(outputFolder, job);

            // MyDocuments folder as fallback for interactive
            if (!job.Profile.AutoSave.Enabled)
                if (string.IsNullOrWhiteSpace(outputFolder))
                    outputFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            return outputFolder;
        }

        protected abstract string ConsiderLastSaveDirectory(string outputFolder, Job job);

        private string ComposeOutputFilename(Job job)
        {
            var outputFilename = ValidName.MakeValidFileName(job.TokenReplacer.ReplaceTokens(job.Profile.FileNameTemplate));

            //"document" as fallback for interactive
            if (!job.Profile.AutoSave.Enabled)
                if (string.IsNullOrWhiteSpace(outputFilename))
                    outputFilename = "document";

            outputFilename += _outputFormatHelper.GetExtension(job.Profile.OutputFormat);

            return outputFilename;
        }
    }
}
