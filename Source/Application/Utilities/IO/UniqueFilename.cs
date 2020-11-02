using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities.IO
{
    /// <summary>
    ///     Creates a file path for a file that does not exist yet. It takes a path and appends a counting number (_2, _3, etc)
    ///     to ensure this in a readable way.
    /// </summary>
    public class UniqueFilename : UniqueFilenameBase
    {
        private readonly IDirectory _directoryWrap;
        private readonly IFile _fileWrap;

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
