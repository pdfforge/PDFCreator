using FluentFTP;

namespace pdfforge.PDFCreator.Utilities.Ftp
{
    public class FtpClientWrap : IFtpClient
    {
        private readonly FtpClient _ftpClient;

        public FtpClientWrap(string host, string userName, string password)
        {
            _ftpClient = new FtpClient(host, userName, password);
        }

        public void Connect()
        {
            _ftpClient.Connect();
        }

        public void Disconnect()
        {
            _ftpClient.Disconnect();
        }

        public bool FileExists(string filePath)
        {
            return _ftpClient.FileExists(filePath);
        }

        public void CreateDirectory(string path)
        {
            _ftpClient.CreateDirectory(path);
        }

        public bool DirectoryExists(string directory)
        {
            return _ftpClient.DirectoryExists(directory);
        }

        public void UploadFile(string localFile, string remoteFile)
        {
            _ftpClient.UploadFile(localFile, remoteFile);
        }
    }
}
