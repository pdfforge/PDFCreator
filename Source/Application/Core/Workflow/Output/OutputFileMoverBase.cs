using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Query;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow.Output
{
    public interface IOutputFileMover
    {
        /// <summary>
        ///     Renames and moves all files from TempOutputFiles to their destination according to
        ///     the FilenameTemplate and stores them in the OutputFiles list.
        ///     For multiple files the FilenameTemplate gets an appendix.
        /// </summary>
        Task MoveOutputFiles(Job job);
    }

    public abstract class OutputFileMoverBase : IOutputFileMover
    {
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);

        protected abstract IUniqueFilenameFactory UniqueFilenameFactory { get; }
        protected abstract IDirectoryHelper DirectoryHelper { get; }
        protected abstract IFile File { get; }

        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected abstract IPathUtil PathUtil { get; }

        private string _outfilebody;

        protected abstract Task<QueryResult<string>> HandleInvalidRootedPath(string filename, OutputFormat outputFormat);

        protected abstract Task<QueryResult<string>> HandleFirstFileFailed(string filename, OutputFormat outputFormat);

        protected abstract HandleCopyErrorResult QueryHandleCopyError(int fileNumber);

        protected abstract bool ApplyUniqueFilename(Job job);

        /// <summary>
        ///     Renames and moves all files from TempOutputFiles to their destination according to
        ///     the FilenameTemplate and stores them in the OutputFiles list.
        ///     For multiple files the FilenameTemplate gets an appendix.
        /// </summary>
        public async Task MoveOutputFiles(Job job)
        {
            Logger.Trace("Moving output files to final location");

            if (!PathUtil.IsValidRootedPath(job.OutputFileTemplate))
            {
                var result = await HandleInvalidRootedPath(job.OutputFileTemplate, job.Profile.OutputFormat);
                if (result.Success == false)
                {
                    throw new AbortWorkflowException("User cancelled retyping invalid rooted path.");
                }
                job.OutputFileTemplate = result.Data;
            }

            _outfilebody = DetermineOutfileBody(job.OutputFileTemplate);

            var outputDirectory = PathSafe.GetDirectoryName(job.OutputFileTemplate);

            DirectoryHelper.CreateDirectory(outputDirectory);

            //Ensure the the first file is the first in TempOutputFiles
            job.TempOutputFiles = job.TempOutputFiles.OrderBy(x => x).ToList();

            int fileNumber = 0;
            foreach (var tempOutputFile in job.TempOutputFiles)
            {
                fileNumber++;

                var extension = PathSafe.GetExtension(tempOutputFile);
                var numberSuffix = DetermineNumWithDigits(job, tempOutputFile);

                var currentOutputFile = _outfilebody + numberSuffix + extension;

                await SemaphoreSlim.WaitAsync();
                try
                {
                    var uniqueFilename = UniqueFilenameFactory.Build(currentOutputFile);
                    if (ApplyUniqueFilename(job))
                    {
                        currentOutputFile = EnsureUniqueFilename(uniqueFilename);
                    }

                    if (!CopyFile(tempOutputFile, currentOutputFile))
                    {
                        var action = QueryHandleCopyError(fileNumber);

                        switch (action)
                        {
                            case HandleCopyErrorResult.Requery:
                                currentOutputFile = await RequeryFilename(job, tempOutputFile, numberSuffix, extension);
                                break;

                            default:
                                currentOutputFile = EnsureUniqueFilename(uniqueFilename);

                                if (!CopyFile(tempOutputFile, currentOutputFile))
                                {
                                    throw new ProcessingException("Error while copying to target file in second attempt. Process gets canceled.", ErrorCode.Conversion_ErrorWhileCopyingOutputFile);
                                }

                                break;
                        }
                    }
                }
                finally
                {
                    SemaphoreSlim.Release();
                }

                DeleteFile(tempOutputFile);
                job.OutputFiles.Add(currentOutputFile);
            }
            job.OutputFiles = job.OutputFiles.OrderBy(x => x).ToList();
        }

        private async Task<string> RequeryFilename(Job job, string tempOutputFile, string numWithDigits, string extension)
        {
            while (true)
            {
                var result = await HandleFirstFileFailed(job.OutputFileTemplate, job.Profile.OutputFormat);

                if (result.Success == false)
                {
                    throw new AbortWorkflowException("User cancelled during retype filename");
                }
                job.OutputFileTemplate = result.Data;
                _outfilebody = DetermineOutfileBody(job.OutputFileTemplate);
                var currentOutputFile = _outfilebody + numWithDigits + extension;

                if (CopyFile(tempOutputFile, currentOutputFile))
                    return currentOutputFile;
            }
        }

        /// <summary>
        ///     Ensure unique filename.
        /// </summary>
        /// <param name="uniqueFilename">The UniqueFilename object that should be used</param>
        /// <returns>unique outputfilename</returns>
        private string EnsureUniqueFilename(IUniquePath uniqueFilename)
        {
            try
            {
                Logger.Debug("Ensuring unique filename for: " + uniqueFilename.OriginalFilename);
                var newFilename = uniqueFilename.CreateUniqueFileName();
                Logger.Debug("Unique filename result: " + newFilename);
                return newFilename;
            }
            catch (PathTooLongException ex)
            {
                throw new ProcessingException(ex.Message, ErrorCode.Conversion_PathTooLong);
            }
        }

        private void DeleteFile(string tempfile)
        {
            try
            {
                File.Delete(tempfile);
            }
            catch (IOException)
            {
                Logger.Warn("Could not delete temporary file \"" + tempfile + "\"");
            }
        }

        /// <summary>
        ///     Copy file with logging and catching of ioException
        /// </summary>
        /// <returns>true if successful</returns>
        private bool CopyFile(string tempFile, string outputFile)
        {
            try
            {
                File.Copy(tempFile, outputFile, true);
                Logger.Debug("Copied output file \"{0}\" \r\nto \"{1}\"", tempFile, outputFile);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Warn("Error while copying to target file.\r\nfrom\"{0}\" \r\nto \"{1}\"\r\n{2}", tempFile, outputFile, ex.Message);
            }
            return false;
        }

        private string DetermineOutfileBody(string outputFilenameTemplate)
        {
            var outputDir = PathSafe.GetDirectoryName(outputFilenameTemplate) ?? "";
            var filenameBase = PathSafe.GetFileNameWithoutExtension(outputFilenameTemplate) ?? "output";
            return PathSafe.Combine(outputDir, filenameBase);
        }

        private string DetermineNumWithDigits(Job job, string tempOutputFile)
        {
            var tempFileBase = PathSafe.GetFileNameWithoutExtension(tempOutputFile) ?? "output";
            var num = tempFileBase.Replace(job.JobTempFileName, "");

            if (job.TempOutputFiles.Count == 1)
                num = "";
            else
            {
                int numValue;
                if (int.TryParse(num, out numValue))
                {
                    var numDigits = (int)Math.Floor(Math.Log10(job.TempOutputFiles.Count) + 1);
                    num = numValue.ToString("D" + numDigits);
                }
            }
            return num;
        }
    }

    public enum HandleCopyErrorResult
    {
        EnsureUniqueFilename,
        Requery
    }
}
