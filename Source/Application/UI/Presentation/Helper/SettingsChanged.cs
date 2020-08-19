using NLog;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface ISettingsChanged
    {
        bool HaveChanged();
    }

    public class SettingsChanged : ISettingsChanged
    {
        private readonly CurrentSettingsProvider _settingsProvider;
        private readonly ILanguageProvider _languageProvider;
        private readonly IGpoSettings _gpoSettings;
        private PdfCreatorSettings _currentSettingsSnapshot;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SettingsChanged(CurrentSettingsProvider settingsProvider,
            ILanguageProvider languageProvider, IGpoSettings gpoSettings, ISettingsManager settingsManager)
        {
            _settingsProvider = settingsProvider;
            _languageProvider = languageProvider;
            _gpoSettings = gpoSettings;
            settingsManager.SettingsSaved += SettingsManager_SettingsSaved;
            _currentSettingsSnapshot = _settingsProvider.Settings.Copy();
        }

        private void SettingsManager_SettingsSaved(object sender, System.EventArgs e)
        {
            _logger.Trace("Updating settings snapshot");
            _currentSettingsSnapshot = _settingsProvider.Settings.Copy();
        }

        public bool HaveChanged()
        {
            var storedSettings = _settingsProvider.Settings;

            if (string.IsNullOrWhiteSpace(_gpoSettings?.Language))
                if (_currentSettingsSnapshot.ApplicationSettings.Language != _languageProvider.CurrentLanguage.Iso2)
                {
                    _logger.Trace("Checking settings for changes: The language has changed");
                    return true;
                }

            var settingsHaveChanged = !_currentSettingsSnapshot.Equals(storedSettings);
            _logger.Trace("Settings have changed: " + settingsHaveChanged);

            return settingsHaveChanged;
        }
    }
}
