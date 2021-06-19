using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OpenFile;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeOpenViewerActionViewModel : OpenViewerActionViewModel
    {
        public DesignTimeOpenViewerActionViewModel()
            : base(
                new DesignTimeTranslationUpdater(),
                new DesignTimeActionLocator(),
                new DesignTimeErrorCodeInterpreter(),
                new DesignTimeCurrentSettingsProvider(),
                new DesignTimeDispatcher(),
                new DesignTimeDefaultSettingsBuilder(),
                new DesignTimeActionOrderHelper(true, false))
        {
        }
    }
}
