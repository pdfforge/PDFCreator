using System;
using System.IO;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities.IO
{
    public interface IUniquePath
    {
        string OriginalFilename { get; }
        string LastUniqueFilename { get; }

        /// <summary>
        ///     Creates a file path for a file that does not exist yet. It takes a path and appends a counting number (_2, _3, etc)
        ///     to ensure this in a readable way.
        /// </summary>
        /// <returns>A unique filename</returns>
        string CreateUniqueFileName();
    }

    public abstract class UniqueFilenameBase : IUniquePath
    {
        private readonly string _directory;
        private readonly string _extension;
        private readonly string _fileBody;

        private readonly IPathUtil _pathUtil;

        // this is the counting number that is appended to the filename
        // starting with 2 to name the first duplicate somename_2
        private int _appendix = 2;

        /// <param name="originalFilename">Original file name</param>
        protected UniqueFilenameBase(string originalFilename, IPathUtil pathUtil)
        {
            if (originalFilename == null)
                throw new ArgumentNullException();

            if (string.IsNullOrWhiteSpace(originalFilename))
                throw new ArgumentException(nameof(originalFilename));

            _pathUtil = pathUtil;

            OriginalFilename = originalFilename;
            LastUniqueFilename = originalFilename;
            _directory = PathSafe.GetDirectoryName(OriginalFilename) ?? "";
            _fileBody = PathSafe.GetFileNameWithoutExtension(OriginalFilename);
            _extension = PathSafe.GetExtension(OriginalFilename);
        }

        public string OriginalFilename { get; }
        public string LastUniqueFilename { get; private set; }

        /// <summary>
        ///     Creates a file path for a file that does not exist yet. It takes a path and appends a counting number (_2, _3, etc)
        ///     to ensure this in a readable way.
        /// </summary>
        /// <returns>A unique filename</returns>
        public string CreateUniqueFileName()
        {
            while (UniqueCondition(LastUniqueFilename))
            {
                LastUniqueFilename = PathSafe.Combine(_directory, _fileBody + "_" + _appendix + _extension);
                _appendix++;
                if (LastUniqueFilename.Length > _pathUtil.MAX_PATH)
                {
                    throw new PathTooLongException("Can not create useful unique filename for too long path: " + LastUniqueFilename);
                }
            }
            return LastUniqueFilename;
        }

        protected abstract bool UniqueCondition(string filename);
    }
}
