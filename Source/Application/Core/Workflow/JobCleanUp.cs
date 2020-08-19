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
                    _directory.Delete(jobTempFolder, true);
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
                    _file.Delete(file.Filename);

                    var folder = Path.GetDirectoryName(file.Filename);
                    DeleteFolderIfEmptyAndNotSpool(folder);
                }
                catch (Exception ex)
                {
                    _logger.Warn("Error while deleting source file: " + ex.Message);
                }
            }
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
                _directory.Delete(folder);
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
                    return;

                _file.Delete(infFile);

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
