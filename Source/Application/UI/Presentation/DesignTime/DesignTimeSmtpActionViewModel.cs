using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailSmtp;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSmtpActionViewModel : SmtpActionViewModel
    {
        public DesignTimeSmtpActionViewModel() : base(new DesignTimeActionLocator(), null, null, null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), new DesignTimeCommandLocator(), new DesignTimeTokenViewModelFactory(), null, new GpoSettingsDefaults(), new DesignTimeSelectFilesUserControlViewModelFactory(), null, null)
        {
        }
    }
}
