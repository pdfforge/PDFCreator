using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSmtpActionViewModel : SmtpActionViewModel
    {
        public DesignTimeSmtpActionViewModel() : base(null, null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null, new DesignTimeCommandLocator(), new DesignTimeTokenViewModelFactory(), null, new GpoSettingsDefaults(), null)
        {
        }
    }
}
