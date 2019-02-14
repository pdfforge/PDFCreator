using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.FTP;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeFtpActionViewModel : FtpActionViewModel
    {
        public DesignTimeFtpActionViewModel() : base(new DesignTimeTranslationUpdater(), null, new DesignTimeCurrentSettingsProvider(), new DesignTimeCommandLocator(), new DesignTimeTokenViewModelFactory(), null, new GpoSettingsDefaults())
        {
        }
    }
}
