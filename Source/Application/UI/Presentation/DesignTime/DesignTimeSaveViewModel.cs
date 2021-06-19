using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSaveViewModel : SaveViewModel
    {
        public DesignTimeSaveViewModel() :
            base(
                new TokenButtonFunctionProvider(new InteractionInvoker(), new OpenFileInteractionHelper(new InteractionInvoker())),
                new DesignTimeCurrentSettingsProvider(),
                new DesignTimeTranslationUpdater(),
                new DesignTimeEditionHelper(),
                new DesignTimeTokenHelper(),
                new DesignTimeTokenViewModelFactory(),
                new DispatcherWrapper(),
                new DesignTimeErrorCodeInterpreter(),
                new DesignTimeProfileChecker(),
                new DesignTimeActionManager())
        { }
    }
}
