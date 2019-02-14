using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeAccountsViewModel : AccountsViewModel
    {
        public DesignTimeAccountsViewModel() :
            base(new CurrentSettingsProvider(new DefaultSettingsProvider()), null, new DesignTimeCommandLocator(), new DesignTimeTranslationUpdater(), new DispatcherWrapper(), null)
        {
            AllAccounts.Add(new SmtpAccount { UserName = "UserName", Server = "SMTP.Server.org" });
            AllAccounts.Add(new DropboxAccount { AccountInfo = "Dropbox Account for UserName" });
            AllAccounts.Add(new FtpAccount { UserName = "UserName", Server = "FTP.Server.org" });
            AllAccounts.Add(new HttpAccount { UserName = "UserName", Url = "HTTP.Server.org" });
            AllAccounts.Add(new TimeServerAccount { UserName = "UserName", Url = "TimeServer.org" });
        }
    }
}
