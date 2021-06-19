using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeOutputFormatPdfViewModel : OutputFormatPdfViewModel
    {
        public DesignTimeOutputFormatPdfViewModel()
            : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), new DesignTimeEditionHelper(), null)
        {
        }
    }
}
