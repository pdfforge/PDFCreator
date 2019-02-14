using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using System;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class LanguageChangedEventArgs : EventArgs
    {
        public LanguageChangedEventArgs(PdfCreatorSettings settings)
        {
            Settings = settings;
        }

        public PdfCreatorSettings Settings { get; }
    }

    public abstract class SettingsProvider : ISettingsProvider
    {
        protected string CurrentLanguage { get; private set; } = "en";
        public PdfCreatorSettings Settings => _settings ?? new PdfCreatorSettings();
        private PdfCreatorSettings _settings;

        public event EventHandler<LanguageChangedEventArgs> LanguageChanged;

        public event EventHandler SettingsChanged;

        public abstract string GetApplicationLanguage();

        public bool CheckValidSettings(PdfCreatorSettings settings)
        {
            return settings.ConversionProfiles.Count > 0;
        }

        public ConversionProfile GetDefaultProfile()
        {
            return Settings.GetProfileByGuid(ProfileGuids.DEFAULT_PROFILE_GUID);
        }

        private void RaiseLanguageChanged(PdfCreatorSettings settings)
        {
            LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(settings));
        }

        public void UpdateSettings(PdfCreatorSettings settings)
        {
            if (!settings.ApplicationSettings.Language.Equals(CurrentLanguage) || _settings == null)
            {
                CurrentLanguage = settings.ApplicationSettings.Language;
                RaiseLanguageChanged(settings);
            }

            _settings = settings.CopyAndPreserveApplicationSettings();

            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class DefaultSettingsProvider : SettingsProvider
    {
        public IGpoSettings GpoSettings => new GpoSettingsDefaults();

        public override string GetApplicationLanguage()
        {
            return CurrentLanguage;
        }
    }
}
