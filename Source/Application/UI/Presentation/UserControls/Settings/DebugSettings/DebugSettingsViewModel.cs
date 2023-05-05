using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class DebugSettingsViewModel : TranslatableViewModelBase<DebugSettingsTranslation>, IWhitelisted, IMountable
    {
        private readonly ICurrentSettings<ApplicationSettings> _applicationSettings;
        private readonly IGpoSettings _gpoSettings;

        public DebugSettingsViewModel(ITranslationUpdater translationUpdater, ICurrentSettings<ApplicationSettings> applicationSettings, IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            _applicationSettings = applicationSettings;
            _gpoSettings = gpoSettings;
        }


        public bool DebugIsDisabled
        {
            get
            {
                if (_applicationSettings?.Settings == null)
                    return false;

                return _gpoSettings?.DisableDebugTab ?? false;
            }
        }

        public void MountView()
        {
            
        }

        public void UnmountView()
        {
        }
    }
}
