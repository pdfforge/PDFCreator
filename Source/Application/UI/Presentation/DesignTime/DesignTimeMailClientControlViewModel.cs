using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailClient;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeMailClientControlViewModel : MailClientControlViewModel
    {
        public DesignTimeMailClientControlViewModel() : base(null, null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), new DesignTimeTokenViewModelFactory(), null, new DesignTimeSelectFilesUserControlViewModelFactory())
        {
        }
    }
}
