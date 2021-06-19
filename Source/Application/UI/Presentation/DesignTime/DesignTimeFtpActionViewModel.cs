using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.FTP;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeFtpActionViewModel : FtpActionViewModel
    {
        public DesignTimeFtpActionViewModel()
            : base(
            new DesignTimeTranslationUpdater(),
            new DesignTimeCurrentSettingsProvider(),
            new DesignTimeCommandLocator(),
            new DesignTimeTokenViewModelFactory(),
            new DesignTimeDispatcher(),
            new GpoSettingsDefaults(),
            new DesignTimeActionLocator(),
            new DesignTimeErrorCodeInterpreter(),
            new DesignTimeDefaultSettingsBuilder(),
            new DesignTimeActionOrderHelper(true, false))
        {
        }
    }
}
