using pdfforge.PDFCreator.Conversion.Actions.Actions.Ftp;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeFtpConnectionTester : IFtpConnectionTester
    {
        public DesignTimeFtpConnectionTester()
        {
        }

        public bool TestFtpConnection(FtpAccount ftpAccount)
        {
            return false;
        }

        public ActionResult CheckAccount(FtpAccount ftpAccount, bool ignoreMissingPassword)
        {
            return new ActionResult();
        }
    }
}
