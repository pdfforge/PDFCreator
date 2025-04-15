using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.EmailWeb;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeMailWebActionViewModel : MailWebActionViewModel
    {
        public DesignTimeMailWebActionViewModel()

            : base(
                new DesignTimeActionLocator(),
                new DesignTimeErrorCodeInterpreter(),
                new DesignTimeCurrentSettingsProvider(),
                new InteractionRequest(),
                new DesignTimeTranslationUpdater(),
                new DesignTimeTokenViewModelFactory(),
                new DesignTimeDispatcher(),
                new DesignTimeSelectFilesUserControlViewModelFactory(),
                new DesignTimeDefaultSettingsBuilder(),
                new DesignTimeActionOrderHelper(),
                new DesignTimeCurrentSettings<Accounts>(),
                new GpoSettingsDefaults(),
                new DesignTimeCommandLocator(),
                new DesignTimeEditionHelper())
        { }
    }
}
