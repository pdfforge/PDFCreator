using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Encrypt;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeEncryptUserControlViewModel : EncryptUserControlViewModel
    {
        public DesignTimeEncryptUserControlViewModel() : base(null, null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), null)
        {
        }
    }
}
