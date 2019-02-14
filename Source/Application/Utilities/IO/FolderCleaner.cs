using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pdfforge.PDFCreator.Utilities.IO
{
    public class CleanUpPathExceptionPairs : Dictionary<string, Exception>
    { }

    public interface IFolderCleaner
    {
        CleanUpPathExceptionPairs Clean(string cleanupFolder, TimeSpan minAge);
    }

    /// <summary>
    ///     Helper class to clean a folder with files older then a specific date
    /// </summary>
    public class FolderCleaner : IFolderCleaner
    {
        private CleanUpPathExceptionPairs _cleanUpPathExceptionPairs;

        /// <summary>
        ///     Clean all files in the given folder. The folder itself will NOT be deleted during cleanup.
        ///     If exceptions occur while cleaning up, they will be returned as CleanUpPathExceptionPairs.
        /// </summary>
        public CleanUpPathExceptionPairs Clean(string cleanupFolder)
        {
            return Clean(cleanupFolder, TimeSpan.Zero);
        }

        /// <summary>
        ///     Clean all files in the given folder. The folder itself will NOT be deleted during cleanup.
        ///     If exceptions occur while cleaning up, they will be returned as CleanUpPathExceptionPairs.
        ///     <param name="minAge">The minimum TimeSpan between file creation date and current time.</param>
        /// </summary>
        public CleanUpPathExceptionPairs Clean(string cleanupFolder, TimeSpan minAge)
        {
            _cleanUpPathExceptionPairs = new CleanUpPathExceptionPairs(); //clear
            try
            {
                if (!Directory.Exists(cleanupFolder))
                    return _cleanUpPathExceptionPairs;

                DoClean(cleanupFolder, minAge);
            }
            catch (Exception ex)
            {
                HandleException(cleanupFolder, ex);
            }

            return _cleanUpPathExceptionPairs;
        }

        private void DoClean(string folder, TimeSpan minAge)
        {
            var folders = new DirectoryInfo(folder).GetDirectories();
            foreach (var item in folders)
            {
                if (Directory.Exists(item.FullName))
                {
                    Clean(item.FullName, minAge);
                    try
                    {
                        if (!Directory.EnumerateFileSystemEntries(item.FullName).Any())
                            DeleteFolder(minAge, item.FullName);
                    }
                    catch (Exception ex)
                    {
                        HandleException(item.FullName, ex);
                    }
                }
            }

            DeleteFiles(folder, minAge);
        }

        private void DeleteFolder(TimeSpan minAge, string subFolder)
        {
            if (Directory.Exists(subFolder))
            {
                try
                {
                    var directory = new DirectoryInfo(subFolder);
                    var folderAge = DateTime.UtcNow - directory.CreationTimeUtc;

                    if (folderAge >= minAge)
                        Directory.Delete(subFolder);
                }
                catch (Exception ex)
                {
                    HandleException(subFolder, ex);
                }
            }
        }

        private void DeleteFiles(string folder, TimeSpan minAge)
        {
            if (Directory.Exists(folder))
            {
                var files = new DirectoryInfo(folder).GetFiles();
                foreach (var item in files)
                {
                    try
                    {
                        var fileAge = DateTime.UtcNow - item.CreationTimeUtc;
                        if (File.Exists(item.FullName) && fileAge >= minAge)
                            File.Delete(item.FullName);
                    }
                    catch (Exception ex)
                    {
                        HandleException(item.FullName, ex);
                    }
                }
            }
        }

        private void HandleException(string path, Exception ex)
        {
            _cleanUpPathExceptionPairs[path] = ex;
        }
    }
}
