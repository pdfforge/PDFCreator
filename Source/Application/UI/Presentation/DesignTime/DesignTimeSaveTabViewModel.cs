using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.Controls;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using SaveTabViewModel = pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.SaveTabViewModel;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSaveTabViewModel : SaveTabViewModel
    {
        public DesignTimeSaveTabViewModel() : base(new TokenButtonFunctionProvider(new InteractionInvoker(), new OpenFileInteractionHelper(new InteractionInvoker())), new DesignTimeCurrentSettingsProvider(), new DesignTimeTranslationUpdater(), new EditionHelper(true, true), new TokenHelper(new DesignTimeTranslationUpdater()), new DesignTimeTokenViewModelFactory(), null)
        {
        }
    }
}
