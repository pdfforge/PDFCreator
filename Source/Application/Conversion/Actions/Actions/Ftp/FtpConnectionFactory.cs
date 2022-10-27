using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Ftp
{
    public interface IFtpConnectionFactory
    {
        IFtpClient BuildConnection(FtpAccount ftpAccount, string password);
    }

    public class FtpConnectionFactory : IFtpConnectionFactory
    {
        public IFtpClient BuildConnection(FtpAccount account, string password)
        {
            var host = account.GetHost();
            var port = account.GetPort();

            if (account.FtpConnectionType == FtpConnectionType.Sftp)
                return new SftpClientWrap(host, port, account.UserName, password, account.PrivateKeyFile, account.AuthenticationType);

            return new FtpClientWrap(host, port, account.UserName, password);
        }
    }
}
