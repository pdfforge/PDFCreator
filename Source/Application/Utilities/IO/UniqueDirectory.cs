using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities.IO
{
    /// <summary>
    ///     Creates a directory that does not exist yet. It takes a path and appends a counting number (_2, _3, etc) to ensure
    ///     this in a readable way.
    /// </summary>
    public interface IUniqueDirectory
    {
        string MakeUniqueDirectory(string path);
    }

    /// <summary>
    ///     Creates a directory that does not exist yet. It takes a path and appends a counting number (_2, _3, etc) to ensure
    ///     this in a readable way.
    /// </summary>
    public class UniqueDirectory : IUniqueDirectory
    {
        private readonly IDirectory _directoryWrap;
        private readonly IFile _fileWrap;

        public UniqueDirectory(IDirectory directory, IFile fileWrap)
        {
            _directoryWrap = directory;
            _fileWrap = fileWrap;
        }

        /// <summary>
        ///     Creates a directory that does not exist yet. It takes a path and appends a counting number (_2, _3, etc) to ensure
        ///     this in a readable way.
        /// </summary>
        /// <returns>The uniqified directory path</returns>
        public string MakeUniqueDirectory(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Argument may not be empty string", nameof(path));

            var directory = PathSafe.GetDirectoryName(path) ?? "";
            var fileBody = PathSafe.GetFileName(path);

            var i = 2;

            while (_directoryWrap.Exists(path) || _fileWrap.Exists(path))
            {
                path = PathSafe.Combine(directory, fileBody + "_" + i);
                i++;
            }

            return path;
        }
    }
}
