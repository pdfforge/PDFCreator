using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeMetadataViewModel : MetadataViewModel
    {
        public DesignTimeMetadataViewModel() :
            base(new DesignTimeTranslationUpdater(),
                new DesignTimeTokenViewModelFactory(),
                new DesignTimeCurrentSettingsProvider(),
                new DispatcherWrapper())
        {
        }
    }
}
