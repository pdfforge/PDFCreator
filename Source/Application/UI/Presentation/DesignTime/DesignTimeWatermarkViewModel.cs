using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions;
using pdfforge.PDFCreator.Utilities.Pdf;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeWatermarkViewModel : WatermarkViewModel
    {
        public DesignTimeWatermarkViewModel() : base(null, new DesignTimeTranslationUpdater(), null, new DesignTimeTokenViewModelFactory(),
            null, new PdfVersionHelper(), new DesignTimeCurrentSettingsProvider(), new DesignTimeActionLocator(),
            null, null, null)
        {
        }
    }
}
