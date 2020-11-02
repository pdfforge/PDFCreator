using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSaveViewModel : SaveViewModel
    {
        public DesignTimeSaveViewModel() : base(new TokenButtonFunctionProvider(new InteractionInvoker(), new OpenFileInteractionHelper(new InteractionInvoker())), new DesignTimeCurrentSettingsProvider(), new DesignTimeTranslationUpdater(), new EditionHelper(true), new TokenHelper(new DesignTimeTranslationUpdater()), new DesignTimeTokenViewModelFactory(), null)
        {
        }
    }
}
