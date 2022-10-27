using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSignatureActionViewModel : SignatureActionViewModel
    {
        public DesignTimeSignatureActionViewModel()
            : base(new DesignTimeOpenFileInteractionHelper(),
                new DesignTimeTranslationUpdater(),
                new DesignTimeCurrentSettingsProvider(),
                new DesignTimeCommandLocator(),
                new DesignTimeTokenViewModelFactory(),
                new DesignTimeDispatcher(),
                new GpoSettingsDefaults(),
                new DesignTimeCurrentSettings<ApplicationSettings>(),
                new DesignTimeInteractionRequest(),
                new DesignTimeActionLocator(),
                new DesignTimeErrorCodeInterpreter(),
                new DesignTimeDefaultSettingsBuilder(),
                new DesignTimeActionOrderHelper(),
                new EditionHelper(Edition.TerminalServer),
                new DesignTimeFontSelectorControlViewModelFactory(),
                null)
        { }
    }
}
