using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab;
using pdfforge.PDFCreator.Utilities.Pdf;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeWatermarkViewModel : WatermarkViewModel
    {
        public DesignTimeWatermarkViewModel() : base(null, new DesignTimeTranslationUpdater(),
            new DesignTimeCurrentSettingsProvider(), null, new DesignTimeTokenViewModelFactory(),
            null, new PdfVersionHelper())
        {
        }
    }
}
