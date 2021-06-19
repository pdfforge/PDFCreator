using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Dropbox;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeDropboxUserControlViewModel : DropboxUserControlViewModel
    {
        public DesignTimeDropboxUserControlViewModel()
            : base(
                new DesignTimeActionLocator(),
                new DesignTimeErrorCodeInterpreter(),
                new DesignTimeTranslationUpdater(),
                new DesignTimeCurrentSettingsProvider(),
                new DesignTimeCommandLocator(),
                new DesignTimeTokenViewModelFactory(),
                new DesignTimeDispatcher(),
                new GpoSettingsDefaults(),
                new DesignTimeDefaultSettingsBuilder(),
                new DesignTimeActionOrderHelper(true, false))
        {
        }
    }
}
