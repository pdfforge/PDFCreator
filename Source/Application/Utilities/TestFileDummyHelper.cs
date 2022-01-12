using System.Text;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface ITestFileDummyHelper
    {
        string CreateFile(string filename, string extension);

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
            filename = ValidName.MakeValidFileName(PathSafe.ChangeExtension(filename, extension));
            var dir = _directoryHelper.CreateTestFileDirectory();
            var testFile = PathSafe.Combine(dir, filename);
            if (!_file.Exists(testFile))
                _file.WriteAllText(testFile, @"PDFCreator Test", Encoding.UTF8);
            return testFile;
        }

        public void CleanUp()
        {
            _directoryHelper.CleanUp();
        }
    }
}
