using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class LanguageSelectionSettingsViewModel : AGeneralSettingsItemControlModel
    {
        private readonly IList<ConversionProfile> _conversionProfiles = new List<ConversionProfile>();
        private readonly ISettingsProvider _settingsProvider;
        private readonly ITranslationHelper _translationHelper;
        private IList<Language> _languages;

        public LanguageSelectionSettingsViewModel(IGpoSettings gpoSettings, ISettingsProvider settingsProvider, ICurrentSettingsProvider currentSettingsProvider, ILanguageProvider languageProvider, ITranslationHelper translationHelper, ITranslationUpdater translationUpdater) :
            base(translationUpdater, currentSettingsProvider, gpoSettings)
        {
            PreviewTranslationCommand = new DelegateCommand(ExecutePreviewTranslation);
            _settingsProvider = settingsProvider;
            _translationHelper = translationHelper;
            Languages = languageProvider.GetAvailableLanguages().ToList();
            SettingsProvider.SettingsChanged += (sender, args) => RaisePropertyChanged(nameof(CurrentLanguage));
        }

        public IList<Language> Languages
        {
            get { return _languages; }
            set
            {
                _languages = value;
                RaisePropertyChanged(nameof(Languages));
            }
        }

        public ICommand PreviewTranslationCommand { get; private set; }

        public bool LanguageIsEnabled
        {
            get
            {
                if (_settingsProvider.Settings?.ApplicationSettings == null)
                    return true;

                return GpoSettings?.Language == null;
            }
        }

        public string CurrentLanguage
        {
            get
            {
                if (_settingsProvider.Settings?.ApplicationSettings == null)
                    return null;

                if (GpoSettings?.Language == null)
                    return _settingsProvider.Settings.ApplicationSettings.Language;

                return GpoSettings.Language;
            }
            set
            {
                _settingsProvider.Settings.ApplicationSettings.Language = value;
            }
        }

        private void ExecutePreviewTranslation(object o)
        {
            var tmpLanguage = Languages.First(l => l.Iso2 == CurrentLanguage);
            _translationHelper.SetTemporaryTranslation(tmpLanguage);
            _translationHelper.TranslateProfileList(_conversionProfiles);

            // Notify about changed properties
            RaisePropertyChanged(nameof(ApplicationSettings));
        }
    }
}