using System;
using System.Collections.Generic;
using System.IO;

namespace PDFCreator.TestUtilities
{
    public static class TempFileHelper
    {
        private static readonly List<string> TempFolderList = new List<string>();

        public static void CleanUp()
        {
            foreach (var folder in TempFolderList)
            {
                try
                {
                    if (Directory.Exists(folder))
                        Directory.Delete(folder, true);
                }
                catch (IOException)
                {
                }
            }
        }

        /// <summary>
        ///     Creates a folder in the TMP path that starts with baseName and has a random part in it and returns its. It ensures
        ///     that the folder did not exist before (at least tries to 10 times)
        ///     For the baseName "test" you might receive "C:\Temp\test_qvvim24t.zjq"
        /// </summary>
        /// <param name="baseName">The base of the resulting folder name</param>
        /// <returns>The full path to the folder</returns>
        public static string CreateTempFolder(string baseName)
        {
            var tempFolder = Path.GetTempPath();

            string tmp = null;

            for (var i = 0; i < 10; i++)
            {
                tmp = Path.Combine(tempFolder, baseName + "_" + Path.GetRandomFileName());
                if (!Directory.Exists(tmp))
                {
                    Directory.CreateDirectory(tmp);
                    TempFolderList.Add(tmp);
                    return tmp;
                }
            }
            return tmp;
        }

        /// <summary>
        ///     Creates a text file in a temporary folder with the content "test"
        /// </summary>
        /// <param name="folderPrefix">A folder prefix to make it easier to identify the test folder</param>
        /// <param name="filename">The Name of the file to create, i.e. "test.txt"</param>
        /// <returns>The full path to the file</returns>
        public static string CreateTempFile(string folderPrefix, string filename)
        {
            return CreateTempFile(folderPrefix, filename, "test" + Guid.NewGuid());
        }

        public static string CreateTempFile(string folderPrefix, string filename, string contents)
        {
            var tempFolder = CreateTempFolder(folderPrefix);
            var tempFile = Path.Combine(tempFolder, filename);

            File.WriteAllText(tempFile, contents);

            return tempFile;
        }
    }
}