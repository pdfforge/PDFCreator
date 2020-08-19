using System;
using SystemInterface.IO;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.Utilities.IO
{
    /// <summary>
    ///     Creates a directory that does not exist yet. It takes a path and appends a counting number (_2, _3, etc) to ensure
    ///     this in a readable way.
    /// </summary>
    public class UniqueDirectory
    {
        private readonly IDirectory _directoryWrap;
        private readonly IFile _fileWrap;
        private string _path;

        public UniqueDirectory(string path) : this(path, new DirectoryWrap(), new FileWrap())
        {
        }

        public UniqueDirectory(string path, IDirectory directory, IFile fileWrap)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Argument may not be empty string", nameof(path));

            _path = path.TrimEnd(' ', '.');

            _directoryWrap = directory;
            _fileWrap = fileWrap;
        }

        /// <summary>
        ///     Creates a directory that does not exist yet. It takes a path and appends a counting number (_2, _3, etc) to ensure
        ///     this in a readable way.
        /// </summary>
        /// <returns>The uniqified directory path</returns>
        public string MakeUniqueDirectory()
        {
            var directory = PathSafe.GetDirectoryName(_path) ?? "";
            var fileBody = PathSafe.GetFileName(_path);

            var i = 2;

            while (_directoryWrap.Exists(_path) || _fileWrap.Exists(_path))
            {
                _path = PathSafe.Combine(directory, fileBody + "_" + i);
                i++;
            }

            return _path;
        }
    }
}
