using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public interface ISelectedProfileProvider
    {
        ConversionProfile SelectedProfile { get; set; }

        ConversionProfile GetProfileByName(string name);

        event PropertyChangedEventHandler SelectedProfileChanged;

        event EventHandler SettingsChanged;
    }

    public interface ICurrentSettingsProvider : ISelectedProfileProvider
    {
        void StoreCurrentSettings();

        void Reset();
    }

    public class CurrentSettingsProvider : ObservableObject, ICurrentSettingsProvider
    {
        private readonly ISettingsProvider _settingsProvider;
        private ConversionProfile _selectedProfile;
        private PdfCreatorSettings _settings;

        public CurrentSettingsProvider(ISettingsProvider settingsProvider)
        {
            _settingsProvider = settingsProvider;
            _settingsProvider.SettingsChanged += (sender, args) =>
            {
                UpdateSettings(true);
            };
        }

        public PdfCreatorSettings Settings
        {
            get
            {
                if (_settings == null)
                    UpdateSettings(false);
                return _settings;
            }
        }

        public Accounts Accounts => Settings.ApplicationSettings.Accounts;

        public ObservableCollection<ConversionProfile> Profiles => Settings?.ConversionProfiles;

        public ObservableCollection<TitleReplacement> TitleReplacements => Settings.ApplicationSettings.TitleReplacement;
        public ObservableCollection<PrinterMapping> PrinterMappings => Settings.ApplicationSettings.PrinterMappings;

        public ConversionProfile SelectedProfile
        {
            get
            {
                if (_selectedProfile == null)
                    UpdateSettings(false);
                return _selectedProfile;
            }
            set
            {
                if (value == null)
                    return;
                _selectedProfile = value;
                RaisePropertyChanged(nameof(SelectedProfile));
                SelectedProfileChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedProfile)));
            }
        }

        public ConversionProfile GetProfileByName(string name)
        {
            return Profiles.FirstOrDefault(p => p.Name == name);
        }

        public void StoreCurrentSettings()
        {
            _settingsProvider.UpdateSettings(Settings);
        }

        public void Reset()
        {
            CloneSettings();
            SelectedProfile = _settings.GetProfileByName(_selectedProfile.Name);
            SettingsChanged?.Invoke(this, EventArgs.Empty);
            RaisePropertyChanged(nameof(Settings));
            RaisePropertyChanged(nameof(Accounts));
            RaisePropertyChanged(nameof(TitleReplacements));
            RaisePropertyChanged(nameof(PrinterMappings));
        }

        public event EventHandler SettingsChanged;

        public event PropertyChangedEventHandler SelectedProfileChanged;

        private void UpdateSettings(bool forceUpdate)
        {
            if (_settingsProvider?.Settings == null)
                return;

            if (_settings == null || forceUpdate)
            {
                CloneSettings();
                var firstProfile = Profiles.FirstOrDefault();
                _selectedProfile = _selectedProfile == null ? firstProfile : Profiles.FirstOrDefault(x => x.Guid == _selectedProfile.Guid) ?? firstProfile;
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CloneSettings()
        {
            _settings = _settingsProvider.Settings.CopyAndPreserveApplicationSettings();
        }
    }
}
