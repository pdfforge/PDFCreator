using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.TabHelper;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Mvvm;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class DebugSettingsViewModel : TranslatableViewModelBase<DebugSettingsTranslation>, ITabViewModel
    {
        private readonly ICurrentSettings<ApplicationSettings> _applicationSettings;
        private readonly IGpoSettings _gpoSettings;
        public string Title => Translation.DebugTabTitle;
        public IconList Icon => IconList.DebugSettings;
        public bool HiddenByGPO => false;
        public bool BlockedByGPO => DebugIsDisabled;
        public bool HasNotSupportedFeatures => false;

        public DebugSettingsViewModel(ITranslationUpdater translationUpdater, ICurrentSettings<ApplicationSettings> applicationSettings, IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            _applicationSettings = applicationSettings;
            _gpoSettings = gpoSettings;
        }

        protected override void OnTranslationChanged()
        {
            RaisePropertyChanged(nameof(Title));
            base.OnTranslationChanged();
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
    }
}
