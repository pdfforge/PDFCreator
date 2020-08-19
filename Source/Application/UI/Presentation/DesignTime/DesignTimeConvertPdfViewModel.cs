using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeConvertPdfViewModel : ConvertPdfViewModel
    {
        public DesignTimeConvertPdfViewModel()
            : base(new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(), new EditionHelper(true), null)
        {
        }
    }
}
