using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Windows.Input;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class LanguageSelectionSettingsViewModel : AGeneralSettingsItemControlModel
    {
        private readonly IList<ConversionProfile> _conversionProfiles = new List<ConversionProfile>();
        private readonly ICurrentSettings<ApplicationSettings> _appSettingsProvider;
        private readonly ITranslationHelper _translationHelper;
        private readonly ICommandLocator _commandLocator;
        private IList<Language> _languages;

        public LanguageSelectionSettingsViewModel(IGpoSettings gpoSettings, ICurrentSettings<ApplicationSettings> appSettingsProvider, ICurrentSettingsProvider currentSettingsProvider, ILanguageProvider languageProvider, ITranslationHelper translationHelper, ITranslationUpdater translationUpdater, ICommandLocator commandLocator) :
            base(translationUpdater, currentSettingsProvider, gpoSettings)
        {
            _appSettingsProvider = appSettingsProvider;
            _translationHelper = translationHelper;
            _commandLocator = commandLocator;
            Languages = languageProvider.GetAvailableLanguages().ToList();
            VisitWebsiteCommand = _commandLocator.GetInitializedCommand<UrlOpenCommand, string>(Urls.PdfforgeTranslationUrl);
            SettingsProvider.SettingsChanged += (sender, args) => RaisePropertyChanged(nameof(CurrentLanguage));
        }

        public IList<Language> Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                RaisePropertyChanged(nameof(Languages));
            }
        }

        public bool LanguageIsEnabled
        {
            get
            {
                if (_appSettingsProvider?.Settings == null)
                    return true;

                return string.IsNullOrWhiteSpace(GpoSettings?.Language);
            }
        }

        public string CurrentLanguage
        {
            get
            {
                if (_appSettingsProvider?.Settings== null)
                    return null;

                if (string.IsNullOrWhiteSpace(GpoSettings?.Language))
                    return _appSettingsProvider.Settings.Language;

                return GpoSettings.Language;
            }
            set
            {
                _appSettingsProvider.Settings.Language = value;

                ExecutePreviewTranslation(null);
            }
        }

        public ICommand VisitWebsiteCommand { get; set; } 

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