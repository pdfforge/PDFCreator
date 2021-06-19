using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Cover;
using pdfforge.PDFCreator.Utilities.Pdf;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeCoverUserControlViewModel : CoverUserControlViewModel
    {
        public DesignTimeCoverUserControlViewModel() : base(
            new DesignTimeActionLocator(),
            null,
            new DesignTimeTranslationUpdater(),
            new DesignTimeCurrentSettingsProvider(),
            null,
            null,
            new PdfVersionHelper(),
            new DesignTimeSelectFilesUserControlViewModelFactory(),
            null,
            null)
        {
        }
    }
}
