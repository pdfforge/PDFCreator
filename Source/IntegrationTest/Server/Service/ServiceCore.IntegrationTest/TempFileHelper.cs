using System.Collections.Generic;
using System.IO;

namespace ServiceCore.IntegrationTest
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

        public static string CreateTempFile(string folderPrefix, string filename, string contents)
        {
            var tempFolder = CreateTempFolder(folderPrefix);
            var tempFile = Path.Combine(tempFolder, filename);

            File.WriteAllText(tempFile, contents);

            return tempFile;
        }
    }
}
