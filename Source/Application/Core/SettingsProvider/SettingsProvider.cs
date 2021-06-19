using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.SettingsProvider
{
    public class LanguageChangedEventArgs : EventArgs
    {
        private readonly ApplicationSettings _appSettings;
        private readonly IEnumerable<ConversionProfile> _profiles;

        public LanguageChangedEventArgs(ApplicationSettings appSettings, IEnumerable<ConversionProfile> profiles)
        {
            _appSettings = appSettings;
            _profiles = profiles;
        }

        public ApplicationSettings AppSettings => _appSettings;

        public IEnumerable<ConversionProfile> Profiles => _profiles;
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
            LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(settings.ApplicationSettings, settings.ConversionProfiles));
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
