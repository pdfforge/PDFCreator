using System.IO;
using System.Runtime.Versioning;
using System.Security.AccessControl;

namespace pdfforge.PDFCreator.Utilities.IO
{
    public interface IDirectoryAccessControl
    {
        DirectorySecurity GetAccessControl(string path);
    }

    public class DirectoryAccessControl : IDirectoryAccessControl
    {
        public DirectorySecurity GetAccessControl(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            return FileSystemAclExtensions.GetAccessControl(directoryInfo);
        }
    }
}
