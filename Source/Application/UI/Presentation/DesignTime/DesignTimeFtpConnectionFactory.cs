using pdfforge.PDFCreator.Conversion.Actions.Actions.Ftp;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeFtpConnectionFactory : IFtpConnectionFactory
    {
        public DesignTimeFtpConnectionFactory()
        { }

        public IFtpClient BuildConnection(FtpAccount ftpAccount, string password)
        {
            return new FtpClientWrap("", null, "", "");
        }
    }
}
