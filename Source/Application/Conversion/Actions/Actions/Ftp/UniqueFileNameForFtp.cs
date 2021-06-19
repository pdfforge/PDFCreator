using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.IO;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Ftp
{
    public class UniqueFilenameForFtp : UniqueFilenameBase
    {
        private readonly IFtpClient _ftpConnection;

        public UniqueFilenameForFtp(string originalFilename, IFtpClient ftpConnection, IPathUtil pathUtil)
            : base(originalFilename, pathUtil)
        {
            _ftpConnection = ftpConnection;
        }

        protected override bool UniqueCondition(string file)
        {
            var fileExists = _ftpConnection.FileExists(file);
            return fileExists;
        }
    }
}
