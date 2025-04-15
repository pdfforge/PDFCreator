using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using DesignTimeTokenViewModelFactory = pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper.DesignTimeTokenViewModelFactory;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Sharepoint
{
    public class DesignTimeSharepointActionViewModel : SharepointActionViewModel
    {
        public DesignTimeSharepointActionViewModel()
            : base(new DesignTimeActionLocator(),
                new DesignTimeErrorCodeInterpreter(),
                new DesignTimeTranslationUpdater(),
                new DesignTimeCurrentSettingsProvider(),
                new DesignTimeDispatcher(),
                new DesignTimeDefaultSettingsBuilder(),
                new DesignTimeActionOrderHelper(), null, null, new DesignTimeTokenViewModelFactory(), new DesignTimeDispatcher(), new GpoSettingsDefaults(), new DesignTimeCommandLocator(), new DesignTimeEditionHelper()){}

    }
}
