using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Background;
using pdfforge.PDFCreator.Utilities.Pdf;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeBackgroundUserControlViewModel : BackgroundUserControlViewModel
    {
        public DesignTimeBackgroundUserControlViewModel() : base(null, new DesignTimeTranslationUpdater(),
            new DesignTimeCurrentSettingsProvider(), null, null,
            null, new PdfVersionHelper())
        {
        }
    }
}
