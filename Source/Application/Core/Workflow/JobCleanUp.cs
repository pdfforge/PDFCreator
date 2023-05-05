using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IJobCleanUp
    {
        void DoCleanUp(string jobTempFolder, IList<SourceFileInfo> sourceFileInfos, string infFile);
    }

    public class JobCleanUp : IJobCleanUp
    {
        private readonly IDirectory _directory;
        private readonly IFile _file;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public JobCleanUp(IDirectory directory, IFile file)
        {
            _directory = directory;
            _file = file;
        }

        public void DoCleanUp(string jobTempFolder, IList<SourceFileInfo> sourceFileInfos, string infFile)
        {
            DeleteTemporaryOutput(jobTempFolder);
            DeleteSourceFiles(sourceFileInfos);
            DeleteInfFile(infFile);
        }

        private void DeleteTemporaryOutput(string jobTempFolder)
        {
            if (!string.IsNullOrEmpty(jobTempFolder) && Path.IsPathRooted(jobTempFolder) &&
                _directory.Exists(jobTempFolder))
            {
                try
                {
                    DeleteDirectory(jobTempFolder, true);
                }
                catch (Exception ex)
                {
                    _logger.Warn("Error while deleting temporary folder: " + ex.Message);
                }
            }
        }

        private void DeleteSourceFiles(IList<SourceFileInfo> sourceFileInfos)
        {
            foreach (var file in sourceFileInfos)
            {
                if (!_file.Exists(file.Filename))
                    continue;

                try
                {
                    DeleteFile(file.Filename);
                    var folder = Path.GetDirectoryName(file.Filename);
                    DeleteFolderIfEmptyAndNotSpool(folder);
                }
                catch (Exception ex)
                {
                    _logger.Warn("Error while deleting source file: " + ex.Message);
                }
            }
        }

        private void DeleteFile(string filePath)
        {
            _logger.Trace("Deleting file " + filePath);
            _file.Delete(filePath);
        }

        private void DeleteDirectory(string path, bool recursive)
        {
            _logger.Trace("Deleting directory " + path);
            _directory.Delete(path, recursive);
        }

        private void DeleteFolderIfEmptyAndNotSpool(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            var name = directoryInfo.Name;

            // ensure the spool folder is never deleted, because doing so can lead to problems
            if (string.Equals("Spool", name, StringComparison.OrdinalIgnoreCase))
                return;

            DeleteFolderIfEmpty(directoryPath);
        }

        private void DeleteFolderIfEmpty(string folder)
        {
            if (DirectoryIsEmpty(folder))
            {
                DeleteDirectory(folder, false);
            }
        }

        private bool DirectoryIsEmpty(string dir)
        {
            return !_directory.EnumerateFileSystemEntries(dir).Any();
        }

        private void DeleteInfFile(string infFile)
        {
            try
            {
                if (!_file.Exists(infFile))
                {
                    _logger.Info("Inf file {0} was already removed!", infFile);
                    return;
                }

                DeleteFile(infFile);

                var folder = Path.GetDirectoryName(infFile);
                DeleteFolderIfEmptyAndNotSpool(folder);
            }
            catch (Exception ex)
            {
                _logger.Warn("Error while deleting job file: " + ex.Message);
            }
        }
    }
}
