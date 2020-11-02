using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities.IO
{
    public interface IUniqueFilenameFactory
    {
        IUniquePath Build(string originalFilename);
    }

    public class UniqueFilenameFactory : IUniqueFilenameFactory
    {
        private readonly IDirectory _directory;
        private readonly IFile _file;
        private readonly IPathUtil _pathUtil;

        public UniqueFilenameFactory(IDirectory directory, IFile file, IPathUtil pathUtil)
        {
            _directory = directory;
            _file = file;
            _pathUtil = pathUtil;
        }

        public IUniquePath Build(string originalFilename)
        {
            return new UniqueFilename(originalFilename, _directory, _file, _pathUtil);
        }
    }
}
