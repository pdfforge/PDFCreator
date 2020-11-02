using NLog;
using System.IO;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public class TempDirectoryHelper : ITempDirectoryHelper
    {
        private readonly IDirectory _directory;
        private readonly IPath _path;
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private string TempDirectoryName => PathSafe.Combine(_path.GetTempPath(), "PDFCreatorTempFiles");

        public string CreateTestFileDirectory()
        {
            var path = TempDirectoryName;

            if (!_directory.Exists(path))
                _directory.CreateDirectory(path);

            return path;
        }

        public TempDirectoryHelper(IDirectory directory, IPath path)
        {
            _directory = directory;
            _path = path;
        }

        public void CleanUp()
        {
            try
            {
                if (!_directory.Exists(TempDirectoryName))
                    return;

                Directory.Delete(TempDirectoryName, true);
            }
            catch (IOException exception)
            {
                Logger.Warn($"Was not able to delete folder : {TempDirectoryName}");
                Logger.Warn(exception);
            }
        }
    }
}
