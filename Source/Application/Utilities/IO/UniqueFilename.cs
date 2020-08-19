using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities.IO
{
    public class UniqueFilePath
    {
        public string Original { get; private set; }
        public string Unique { get; private set; }

        public UniqueFilePath(string original, string unique)
        {
            Original = original;
            Unique = unique;
        }
    }

    public interface IUniqueFilePathBuilder
    {
        UniqueFilePath Build(string originalFilePath);
    }

    public class UniqueFilePathBuilder : IUniqueFilePathBuilder
    {
        private readonly IDirectory _directory;
        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;

        public UniqueFilePathBuilder(IDirectory directory, IFile file, IPathUtil pathUtil)
        {
            _directory = directory;
            _file = file;
            _pathUtil = pathUtil;
        }

        public UniqueFilePath Build(string originalFilePath)
        {
            var uniqueFileName = new UniqueFilename(originalFilePath, _directory, _file, _pathUtil);

            return new UniqueFilePath(uniqueFileName.OriginalFilename, uniqueFileName.LastUniqueFilename);
        }
    }

    /// <summary>
    ///     Creates a file path for a file that does not exist yet. It takes a path and appends a counting number (_2, _3, etc)
    ///     to ensure this in a readable way.
    /// </summary>
    public class UniqueFilename : UniqueFilenameBase
    {
        private readonly IDirectory _directoryWrap;
        private readonly IFile _fileWrap;

        /// <param name="originalFilename">Original file name</param>
        public UniqueFilename(string originalFilename, IDirectory directory, IFile file, IPathUtil pathUtil) : base(originalFilename, pathUtil)
        {
            _directoryWrap = directory;
            _fileWrap = file;
        }

        protected override bool UniqueCondition(string filename)
        {
            return _fileWrap.Exists(filename) || _directoryWrap.Exists(filename);
        }
    }
}
