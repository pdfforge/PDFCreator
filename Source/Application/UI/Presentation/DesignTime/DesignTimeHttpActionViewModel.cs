using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.HTTP;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeHttpActionViewModel : HttpActionViewModel
    {
        public DesignTimeHttpActionViewModel()
            : base(
                new DesignTimeTranslationUpdater(),
                new DesignTimeActionLocator(),
                new DesignTimeErrorCodeInterpreter(),
                new DesignTimeCurrentSettingsProvider(),
                new DesignTimeCommandLocator(),
                new DesignTimeDispatcher(),
                new GpoSettingsDefaults(),
                new DesignTimeDefaultSettingsBuilder(),
                new DesignTimeActionOrderHelper(true, false))
        {
        }
    }
}
