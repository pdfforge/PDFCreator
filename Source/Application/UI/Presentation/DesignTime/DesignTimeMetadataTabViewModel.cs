using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeMetadataTabViewModel : MetadataViewModel
    {
        public DesignTimeMetadataTabViewModel() :
            base(new DesignTimeTranslationUpdater(),
                new TokenHelper(new DesignTimeTranslationUpdater()),
                new DesignTimeTokenViewModelFactory(),
                new DesignTimeCurrentSettingsProvider(),
                new DispatcherWrapper())
        {
        }
    }
}
