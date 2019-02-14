using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.FolderProvider;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Utilities.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.DirectConversion
{
    public interface IDirectConversionInfFileHelper
    {
        /// <summary>
        ///     Create unique job folder in spool folder and copy ps file to it.
        ///     Create inf file from ps file.
        /// </summary>
        /// <returns>inf file in spool folder</returns>
        string TransformToInfFile(string directConversionFile);

        string TransformToInfFile(string directConversionFile, AppStartParameters appStartParameters);

        string TransformToInfFileWithMerge(IList<string> directConversionFiles, AppStartParameters appStartParameters);
    }

    public class DirectConversionInfFileHelper : IDirectConversionInfFileHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDirectConversionHelper _directConversionHelper;
        private readonly IJobInfoManager _jobInfoManager;
        private readonly ISpoolerProvider _spoolerProvider;
        private readonly IFile _file;
        private readonly IDirectory _directory;
        private readonly IPath _path;

        public DirectConversionInfFileHelper(
            IDirectConversionHelper directConversionHelper,
            IJobInfoManager jobInfoManager,
            ISpoolerProvider spoolerProvider,
            IFile file,
            IDirectory directory,
            IPath path
            )
        {
            _directConversionHelper = directConversionHelper;
            _jobInfoManager = jobInfoManager;
            _spoolerProvider = spoolerProvider;
            _file = file;
            _directory = directory;
            _path = path;
        }

        public string TransformToInfFile(string directConversionFile, AppStartParameters appStartParameters)
        {
            return TransformToInfFileWithMerge(new List<string> { directConversionFile }, appStartParameters);
        }

        public string TransformToInfFile(string directConversionFile)
        {
            return TransformToInfFileWithMerge(new List<string> { directConversionFile }, new AppStartParameters());
        }

        /// <summary>
        ///     Create unique job folder in spool folder and copy ps file to it.
        ///     Create inf file from ps file.
        /// </summary>
        /// <returns>inf file in spool folder</returns>
        public string TransformToInfFileWithMerge(IList<string> directConversionFiles, AppStartParameters appStartParameters)
        {
            if (directConversionFiles.Count <= 0)
                return "";

            foreach (var file in directConversionFiles)
            {
                if (string.IsNullOrWhiteSpace(file))
                    return "";

                if (!_file.Exists(file))
                {
                    Logger.Error("The file \"" + file + "\" does not exist.");
                    return "";
                }
            }

            string jobFolder;
            try
            {
                jobFolder = CreateJobFolderInSpool(directConversionFiles[0]);
            }
            catch (Exception ex)
            {
                Logger.Error("Error while creating spool directory for ps-job:\r\n" + ex.Message);
                return "";
            }

            try
            {
                var jobFolderFile = CopyFilesToJobFolder(jobFolder, directConversionFiles);
                return CreateInfFile(jobFolderFile, jobFolder, appStartParameters);
            }
            catch (Exception ex)
            {
                Logger.Error("Error while coping ps-file in spool folder:\r\n" + ex.Message);
                _directory.Delete(jobFolder, true); //Delete created folder and files
                return "";
            }
        }

        private string CreateJobFolderInSpool(string file)
        {
            var psFilename = PathSafe.GetFileName(file);
            if (psFilename.Length > 23)
                psFilename = psFilename.Substring(0, 23);
            var jobFolder = PathSafe.Combine(_spoolerProvider.SpoolFolder, psFilename);
            jobFolder = new UniqueDirectory(jobFolder).MakeUniqueDirectory();
            _directory.CreateDirectory(jobFolder);
            Logger.Trace("Created spool directory for ps-file job: " + jobFolder);

            return jobFolder;
        }

        private class JobFolderFile
        {
            public JobFolderFile(string originalFilePath, string fileInJobFolder)
            {
                OriginalFilePath = originalFilePath;
                FileInJobFolder = fileInJobFolder;
            }

            public string OriginalFilePath { get; set; }
            public string FileInJobFolder { get; set; }
        }

        private List<JobFolderFile> CopyFilesToJobFolder(string jobFolder, IList<string> directConversionFiles)
        {
            var filesInJobFolder = new List<JobFolderFile>();
            foreach (var originalFile in directConversionFiles)
            {
                var fileInJobFolder = CopyFileToJobFolder(jobFolder, originalFile);
                var jobFolderFile = new JobFolderFile(originalFile, fileInJobFolder);
                filesInJobFolder.Add(jobFolderFile);
            }
            return filesInJobFolder;
        }

        private string CopyFileToJobFolder(string jobFolder, string file)
        {
            var shortUniqueFilename = _path.GetRandomFileName();
            var extension = PathSafe.GetExtension(file);
            var psFileInJobFolder = PathSafe.Combine(jobFolder, shortUniqueFilename + extension);
            _file.Copy(file, psFileInJobFolder);
            Logger.Debug("Copied direct conversion file to spool folder: " + psFileInJobFolder);

            return psFileInJobFolder;
        }

        private string CreateInfFile(IList<JobFolderFile> jobFolderFiles, string jobFolder, AppStartParameters appStartParameters)
        {
            var firstDirectConversionFile = jobFolderFiles[0].OriginalFilePath;

            var fileName = PathSafe.GetFileName(firstDirectConversionFile);
            var shortFileName = fileName;
            if (fileName.Length > 12)
                shortFileName = fileName.Substring(0, 12);

            var infFile = PathSafe.Combine(jobFolder, shortFileName + ".inf");

            var jobInfo = new JobInfo();

            foreach (var jobFolderFile in jobFolderFiles)
            {
                var sfi = CreateSourceFileInfo(jobFolderFile, appStartParameters);
                jobInfo.SourceFiles.Add(sfi);
            }

            _jobInfoManager.SaveToInfFile(jobInfo, infFile);
            Logger.Debug("Created inf-file for ps-file: " + infFile);

            return infFile;
        }

        private SourceFileInfo CreateSourceFileInfo(JobFolderFile jobFolderFile, AppStartParameters appStartParameters)
        {
            var sourceFileInfo = new SourceFileInfo();
            sourceFileInfo.Filename = jobFolderFile.FileInJobFolder;
            sourceFileInfo.Author = Environment.UserName;
            sourceFileInfo.ClientComputer = Environment.MachineName.Replace("\\", "");
            sourceFileInfo.Copies = 1;
            sourceFileInfo.DocumentTitle = PathSafe.GetFileNameWithoutExtension(jobFolderFile.OriginalFilePath);
            sourceFileInfo.OriginalFilePath = jobFolderFile.OriginalFilePath;
            sourceFileInfo.JobCounter = 0;
            sourceFileInfo.JobId = 0;
            sourceFileInfo.PrinterParameter = appStartParameters.Printer;
            sourceFileInfo.ProfileParameter = appStartParameters.Profile;
            sourceFileInfo.OutputFileParameter = appStartParameters.OutputFile;
            sourceFileInfo.SessionId = Process.GetCurrentProcess().SessionId;
            sourceFileInfo.TotalPages = _directConversionHelper.GetNumberOfPages(jobFolderFile.FileInJobFolder);
            sourceFileInfo.Type = JobType.PsJob;
            sourceFileInfo.WinStation = Environment.GetEnvironmentVariable("SESSIONNAME");

            return sourceFileInfo;
        }
    }
}
