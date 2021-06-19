using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.RunProgram;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeScriptUserControlViewModel : ScriptUserControlViewModel
    {
        public DesignTimeScriptUserControlViewModel()
            : base(
                new DesignTimeTranslationUpdater(),
                null,
                null,
                new DesignTimeTokenHelper(),
                new DesignTimeTokenViewModelFactory(),
                new DesignTimeDispatcher(),
                new DesignTimeActionLocator(),
                new DesignTimeErrorCodeInterpreter(),
                new DesignTimeCurrentSettingsProvider(),
                new DesignTimeDefaultSettingsBuilder(),
                new DesignTimeActionOrderHelper(true, false))
        {
        }
    }
}
