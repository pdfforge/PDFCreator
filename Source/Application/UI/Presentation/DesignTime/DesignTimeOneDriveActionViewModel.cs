using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OneDrive;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeOneDriveActionViewModel : OneDriveActionViewModel
    {
        public DesignTimeOneDriveActionViewModel()
            : base(new DesignTimeActionLocator(),
                new DesignTimeErrorCodeInterpreter(),
                new DesignTimeTranslationUpdater(),
                new DesignTimeCurrentSettingsProvider(),
                new DesignTimeDispatcher(),
                new DesignTimeDefaultSettingsBuilder(),
                new DesignTimeActionOrderHelper(),
                new DesignTimeTokenViewModelFactory(),
                new GpoSettingsDefaults(),
                new DesignTimeCommandLocator(),
                new DesignTimeEditionHelper()){}
    }
}
