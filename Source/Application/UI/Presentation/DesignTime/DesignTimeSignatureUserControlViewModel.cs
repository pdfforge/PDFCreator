using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Sign;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSignatureUserControlViewModel : SignatureUserControlViewModel
    {
        public DesignTimeSignatureUserControlViewModel() : base(null, null, new DesignTimeTranslationUpdater(), new DesignTimeCurrentSettingsProvider(),
                                                                new DesignTimeCommandLocator(), new SignaturePasswordCheck(),
                                                                null, new DesignTimeTokenViewModelFactory(),
                                                                null, new GpoSettingsDefaults(), new DesignTimeSigningPositionUnitConverterFactory(),
                                                                    new DesignTimeCurrentSettings<ApplicationSettings>(), new HashUtil(), new InteractionRequest())
        {
        }
    }
}
