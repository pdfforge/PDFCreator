using System.Diagnostics;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IFileVersionInfoHelper
    {
        string GetFileVersion(string fileName);
    }

    public class FileVersionInfoHelper : IFileVersionInfoHelper
    {
        public string GetFileVersion(string fileName)
        {
            return FileVersionInfo.GetVersionInfo(fileName).FileVersion;
        }
    }
}
