using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimePathUtil : IPathUtil
    {
        public DesignTimePathUtil()
        {
        }

        public int MAX_PATH { get; }
        public string ELLIPSIS { get; }

        public string CheckAndShortenTooLongPath(string filePath, int maxLength)
        {
            return "";
        }

        public string CheckAndShortenTooLongPath(string filePath)
        {
            return "";
        }

        public bool DirectoryIsEmpty(string path)
        {
            return false;
        }

        public bool CheckWritability(string directory)
        {
            return false;
        }

        public bool IsValidRootedPath(string path)
        {
            return false;
        }

        public PathUtilStatus IsValidRootedPathWithResponse(string path)
        {
            return PathUtilStatus.InvalidRootedPath;
        }

        public bool IsValidFilename(string fileName)
        {
            return false;
        }
    }
}
