using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailClient;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeMailClientControlViewModel : MailClientControlViewModel
    {
        public DesignTimeMailClientControlViewModel()
            : base(
                new DesignTimeActionLocator(),
                new DesignTimeErrorCodeInterpreter(),
                new DesignTimeCurrentSettingsProvider(),
                new InteractionRequest(),
                null,
                new DesignTimeTranslationUpdater(),
                new DesignTimeTokenViewModelFactory(),
                new DesignTimeDispatcher(),
                new DesignTimeSelectFilesUserControlViewModelFactory(),
                new DesignTimeDefaultSettingsBuilder(),
                new DesignTimeActionOrderHelper(true, false))
        {
        }
    }
}
