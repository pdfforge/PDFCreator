using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Utilities.Ftp
{
    public interface IFtpConnectionFactory
    {
        IFtpClient BuildConnection(FtpAccount ftpAccount, string password);
    }

    public class FtpConnectionFactory : IFtpConnectionFactory
    {
        public IFtpClient BuildConnection(FtpAccount account, string password)
        {
            if (account.FtpConnectionType == FtpConnectionType.Sftp)
                return new SftpClientWrap(account.Server, account.UserName, password, account.PrivateKeyFile, account.AuthenticationType);

            return new FtpClientWrap(account.Server, account.UserName, password);
        }
    }
}
