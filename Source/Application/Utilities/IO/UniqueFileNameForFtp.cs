using IFtpClient = pdfforge.PDFCreator.Utilities.Ftp.IFtpClient;

namespace pdfforge.PDFCreator.Utilities.IO
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
