using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimePasswordOverlayViewModel : PasswordOverlayViewModel
    {
        public DesignTimePasswordOverlayViewModel() : base(new DesignTimeTranslationUpdater())
        {
        }
    }
}
