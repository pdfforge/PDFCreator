using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSigningViewModel : SigningViewModel
    {
        public DesignTimeSigningViewModel()
            : base(null,
                new DesignTimeTranslationUpdater(),
                new DesignTimeCurrentSettingsProvider(),
                new DesignTimeCommandLocator(),
                new DesignTimeTokenViewModelFactory(),
                null,
                new GpoSettingsDefaults(),
                new DesignTimeSigningPositionUnitConverterFactory(),
                new DesignTimeCurrentSettings<ApplicationSettings>(),
                null,
                new DesignTimeActionLocator(),
                new DesignTimeErrorCodeInterpreter(),
                null,
                null)
        { }
    }
}
