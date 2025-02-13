using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace pdfforge.PDFCreator.Utilities
{
    public static class ValidName
    {
        private static readonly string InvalidFileCharRegex = $@"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]+";

        private static readonly string InvalidPathCharRegex = $@"[{Regex.Escape(new string(Path.GetInvalidPathChars()))}*/?]+";
        private static readonly string InvalidWebPathCharRegex = $@"[{Regex.Escape(new string(Path.GetInvalidPathChars()))}*?]+";

        private static readonly string InvalidFtpCharRegex = $@"/\\|[{Regex.Escape(new string(Path.GetInvalidPathChars()) + ":*?")}]+";

        private static readonly char[] InvalidDropBoxChars = new[] { ':', '?', '*', '|', '"', '*', '.' };

        public static string MakeValidFileName(string name)
        {
            return Regex.Replace(name, InvalidFileCharRegex, "_");
        }

        public static string MakeValidFolderName(string name)
        {
            var tmp = Regex.Replace(name, InvalidPathCharRegex, "_");
            var sanitized = new StringBuilder();

            for (var i = 0; i < tmp.Length; i++)
            {
                var c = tmp[i];

                if (i != 1 && c == ':')
                    c = '_';

                sanitized.Append(c);
            }

            return sanitized.ToString();
        }

        public static string MakeValidFtpPath(string path)
        {
            return Regex.Replace(path, InvalidFtpCharRegex, "_");
        }

        public static bool IsValidFtpPath(string path)
        {
            return !Regex.IsMatch(path, InvalidFtpCharRegex);
        }

        public static bool IsValidWebPath(string name)
        {
            var containsABadCharacter = new Regex(InvalidWebPathCharRegex);
            return !containsABadCharacter.IsMatch(name);
        }

        //Todo: Check if this is obsolete and can be replaced by PathUtil.IsValidRootedPath
        public static bool IsValidPath(string name)
        {
            var containsABadCharacter = new Regex(InvalidPathCharRegex);
            if (containsABadCharacter.IsMatch(name))
            {
                return false;
            }

            // other checks for UNC, drive-path format, etc
            return true;
        }

        public static bool IsValidDropboxFolder(string folder)
        {
            return folder.IndexOfAny(InvalidDropBoxChars) == -1;
        }
    }
}
