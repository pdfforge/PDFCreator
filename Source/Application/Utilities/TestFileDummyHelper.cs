using System.Collections.Generic;
using System.Text;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface ITestFileDummyHelper
    {
        string CreateFile(string filename, string extension);

        List<string> CreateFileList(string filename, string extension, int numberOfFiles);

        void CleanUp();
    }

    public class TestFileDummyHelper : ITestFileDummyHelper
    {
        private readonly IFile _file;
        private readonly ITempDirectoryHelper _directoryHelper;

        public TestFileDummyHelper(IFile file, ITempDirectoryHelper directoryHelper)
        {
            _file = file;
            _directoryHelper = directoryHelper;
        }

        public string CreateFile(string filename, string extension)
        {
            filename = PathSafe.ChangeExtension(filename, extension);
            var dir = _directoryHelper.CreateTestFileDirectory();
            var testFile = PathSafe.Combine(dir, filename);
            if (!_file.Exists(testFile))
                _file.WriteAllText(testFile, @"PDFCreator Test", Encoding.UTF8);
            return testFile;
        }

        public List<string> CreateFileList(string filename, string extension, int numberOfFiles)
        {
            var fileList = new List<string>();

            if (numberOfFiles <= 0)
                return fileList;

            var firstTestFile = CreateFile(filename, extension);
            fileList.Add(firstTestFile);

            for (var i = 2; i <= numberOfFiles; i++)
            {
                var testFile = CreateFile(filename + " " + i, extension);
                fileList.Add(testFile);
            }

            return fileList;
        }

        public void CleanUp()
        {
            _directoryHelper.CleanUp();
        }
    }
}
