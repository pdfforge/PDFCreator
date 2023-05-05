using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface.ImagesToPdf;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public interface IDirectImageConversionHelper
    {
        string TransformToInfFileDirectImageConversion(IList<string> directConversionFiles, AppStartParameters appStartParameters = null);
    }

    public class DirectImageConversionHelper : IDirectImageConversionHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IJobInfoManager _jobInfoManager;
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IJobFolderBuilder _jobFolderBuilder;
        private readonly IImagesToPdf _imagesToPdf;
        private readonly IAppSettingsProvider _appSettings;

        public DirectImageConversionHelper(
            IJobInfoManager jobInfoManager,
            IFile file,
            IDirectory directory,
            IJobFolderBuilder jobFolderBuilder,
            IImagesToPdf imagesToPdf,
            IAppSettingsProvider appSettings
        )
        {
            _jobInfoManager = jobInfoManager;
            _file = file;
            _directory = directory;
            _jobFolderBuilder = jobFolderBuilder;
            _imagesToPdf = imagesToPdf;
            _appSettings = appSettings;
        }

        /// <summary>
        /// Converts images to pdf
        /// </summary>
        /// <param name="directConversionFiles"></param>
        /// <param name="appStartParameters"></param>
        /// <returns></returns>
        public string TransformToInfFileDirectImageConversion(IList<string> directConversionFiles, AppStartParameters appStartParameters = null)
        {
            string jobFolder;
            try
            {
                jobFolder = _jobFolderBuilder.CreateJobFolderInSpool(directConversionFiles[0]);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while creating directory for direct conversion of images: ");
                return "";
            }

            try
            {
                var outputFile = CreateOutputFile(jobFolder, directConversionFiles[0]);

                _imagesToPdf.ConvertImage2Pdf(directConversionFiles, _appSettings.Settings, outputFile);

                var infFile = CreateInfFilefDirectImageConversion(outputFile, jobFolder, appStartParameters);

                return infFile;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error while creating the new pdf document: ");
                _directory.Delete(jobFolder, true); //Delete created folder and files
                return "";
            }
        }

        /// <summary>
        /// Create the inf file of the created pdf file
        /// </summary>
        /// <param name="originalFile"></param>
        /// <param name="jobFolder"></param>
        /// <param name="appStartParameters"></param>
        /// <returns></returns>
        private string CreateInfFilefDirectImageConversion(string originalFile, string jobFolder, AppStartParameters appStartParameters)
        {
            var fileName = PathSafe.GetFileName(originalFile);
            if (fileName.Length > 12)
                fileName.Substring(0, 12);

            var infFile = PathSafe.Combine(jobFolder, fileName + ".inf");

            var jobInfo = new JobInfo();

            var sfi = CreateSourceFileInfoPdf(originalFile, jobFolder, appStartParameters);
            jobInfo.SourceFiles.Add(sfi);

            _jobInfoManager.SaveToInfFile(jobInfo, infFile);

            Logger.Debug("Created inf-file for pdf-file: " + infFile);

            return infFile;
        }

        /// <summary>
        /// Creates source info for the pdf file
        /// </summary>
        /// <param name="originalFile"></param>
        /// <param name="jobFolder"></param>
        /// <param name="appStartParameters"></param>
        /// <returns></returns>
        private SourceFileInfo CreateSourceFileInfoPdf(string originalFile, string jobFolder, AppStartParameters appStartParameters)
        {
            var sourceFileInfo = new SourceFileInfo();
            sourceFileInfo.Filename = originalFile;
            sourceFileInfo.Author = Environment.UserName;
            sourceFileInfo.ClientComputer = Environment.MachineName.Replace("\\", "");
            sourceFileInfo.Copies = 1;
            sourceFileInfo.DocumentTitle = PathSafe.GetFileNameWithoutExtension(originalFile);
            sourceFileInfo.OriginalFilePath = originalFile;
            sourceFileInfo.PrintedAt = DateTime.Now;
            sourceFileInfo.JobCounter = 0;
            sourceFileInfo.JobId = 0;
            sourceFileInfo.PrinterParameter = appStartParameters.Printer;
            sourceFileInfo.ProfileParameter = appStartParameters.Profile;
            sourceFileInfo.OutputFileParameter = appStartParameters.OutputFile;
            sourceFileInfo.SessionId = Process.GetCurrentProcess().SessionId;
            sourceFileInfo.Type = JobType.PsJob;
            sourceFileInfo.WinStation = Environment.GetEnvironmentVariable("SESSIONNAME");

            return sourceFileInfo;
        }

        /// <summary>
        /// Create the output file path
        /// </summary>
        /// <param name="jobFolder"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private string CreateOutputFile(string jobFolder, string file)
        {
            var fileName = PathSafe.GetFileNameWithoutExtension(file);
            var outputFile = PathSafe.Combine(jobFolder, fileName + ".pdf");
            _file.Copy(file, outputFile);
            Logger.Debug("Created direct conversion file to folder: " + outputFile);
            return outputFile;
        }
    }
}
