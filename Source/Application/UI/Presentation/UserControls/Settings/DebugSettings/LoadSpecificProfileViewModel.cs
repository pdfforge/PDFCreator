
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using SystemInterface.IO;


namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public abstract class LoadSpecificProfileViewModelBase : TranslatableViewModelBase<LoadSpecificProfileTranslation>, IMountable, IInteractionAware
    {
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private readonly ICurrentSettings<ApplicationSettings> _applicationSettingsProvider;
        private readonly IIniSettingsAssistant _iniSettingsAssistant;

        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        public DelegateCommand ChooseIniFileCommand { get; private set; }
        public AsyncCommand AddToSettingsCommand { get; private set; }
        public DelegateCommand SelectedProfileChangedCommand { get; private set; }

        public ICommand SaveChangedSettingsCommand { get; }

        public IList<ProfileSelection> ProfileSelections { get; set; } = new List<ProfileSelection>();

        public string IniFile { get; set; }

        public LoadSpecificProfileViewModelBase(
            IOpenFileInteractionHelper openFileInteractionHelper,
            ICommandLocator commandLocator,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
            ICurrentSettings<ApplicationSettings> applicationSettingsProvider,
            IIniSettingsAssistant iniSettingsAssistant,
            ITranslationUpdater translationUpdater
        ) : base(translationUpdater)
        {
            _profilesProvider = profilesProvider;
            _applicationSettingsProvider = applicationSettingsProvider;
            _iniSettingsAssistant = iniSettingsAssistant;
            _openFileInteractionHelper = openFileInteractionHelper;
            ChooseIniFileCommand = new DelegateCommand(ChooseIniFileExecute);
            AddToSettingsCommand = new AsyncCommand(AddToSettingsExecute, AddToSettingsCanExecute);
            SelectedProfileChangedCommand = new DelegateCommand(o => AddToSettingsCommand.RaiseCanExecuteChanged());
            SaveChangedSettingsCommand = commandLocator.GetCommand<ISaveChangedSettingsCommand>();
        }

        private bool AddToSettingsCanExecute(object obj)
        {
            return ProfileSelections.Where(selection => selection.IsSelected).ToList().Any();
        }

        private async Task AddToSettingsExecute(object obj)
        {
            foreach (var profileSelection in ProfileSelections)
            {
                if(!profileSelection.IsSelected)
                    continue;

                var existingProfile = _profilesProvider.Settings.FirstOrDefault(p => p.Guid == profileSelection.Profile.Guid);
                if (existingProfile != null)
                    existingProfile.ReplaceWith(profileSelection.Profile);
                else
                    _profilesProvider.Settings.Add(profileSelection.Profile);

                //Remove existing mappings for selected profile guid
                var existingPrinterMappings = _applicationSettingsProvider.Settings.PrinterMappings
                    .Where(pm => pm.ProfileGuid == profileSelection.Profile.Guid)
                    .ToList();
                foreach (var existingPrinterMapping in existingPrinterMappings)
                {
                    _applicationSettingsProvider.Settings.PrinterMappings.Remove(existingPrinterMapping);
                }

                //Add or remap printer mappings selected profile guid 
                foreach (var printerMapping in profileSelection.PrinterMappings)
                {
                    var existingPrinterMapping = _applicationSettingsProvider.Settings.PrinterMappings
                        .FirstOrDefault(pm => pm.PrinterName == printerMapping.PrinterName);
                    if (existingPrinterMapping != null)
                        existingPrinterMapping.ProfileGuid = printerMapping.ProfileGuid;
                    else
                        _applicationSettingsProvider.Settings.PrinterMappings.Add(printerMapping);
                }
            }

            await _iniSettingsAssistant.SyncPrinterMappingWithInstalledPrintersQuery(_applicationSettingsProvider.Settings.PrinterMappings);

            SaveChangedSettingsCommand.Execute(null);
            FinishInteraction?.Invoke();
        }

        protected abstract (IList<ConversionProfile> profiles, IList<PrinterMapping> printerMappings) LoadFromIniFile(string iniFile);

        private void ChooseIniFileExecute(object obj)
        {
            var title = Translation.SelectIniFile;
            var filter = Translation.PdfCreatorSettingsFiles + @" (*.ini)|*.ini|"
                         + Translation.AllFiles
                         + @" (*.*)|*.*";

            var interactionResult = _openFileInteractionHelper.StartOpenFileInteraction(IniFile, title, filter);

            interactionResult.MatchSome(s =>
            {
                IniFile = s;
                RaisePropertyChanged(nameof(IniFile));

                var (profiles, printerMappings) = LoadFromIniFile(s);

                ProfileSelections = profiles
                    .Select(p => new ProfileSelection
                    {
                        Profile = p, 
                        PrinterMappings = GetPrinterMappings(p, printerMappings),
                        IsSelected = !ProfileExists(p)
                    })
                    .OrderByDescending(p => p.IsSelected)
                    .ToList();

                RaisePropertyChanged(nameof(ProfileSelections));
                AddToSettingsCommand.RaiseCanExecuteChanged();
            });
        }

        private IList<PrinterMapping> GetPrinterMappings(ConversionProfile profile, IList<PrinterMapping> printerMappings)
        {
            return printerMappings.Where(pm => pm.ProfileGuid == profile.Guid).ToList();
        }

        private bool ProfileExists(ConversionProfile profile)
        {
            return _profilesProvider.Settings.FirstOrDefault(currentProfile => currentProfile.Guid == profile.Guid) != null;
        }

        public void MountView()
        {
        }

        public void UnmountView()
        {
        }

        public void SetInteraction(IInteraction interaction)
        {
        }

        public Action FinishInteraction { get; set; }
        
        public abstract string Title { get; }
        public abstract string Description { get; }
    }

    public class ProfileSelection
    {
        public bool IsSelected { get; set; } = false;
        public ConversionProfile Profile { get; set; }
        public IList<PrinterMapping> PrinterMappings { get; set; }
    }

    public class LoadSpecificProfileViewModel : LoadSpecificProfileViewModelBase
    {
        private readonly IIniSettingsLoader _iniSettingsLoader;

        public LoadSpecificProfileViewModel(IIniSettingsLoader iniSettingsLoader, IOpenFileInteractionHelper openFileInteractionHelper, 
            ICommandLocator commandLocator, ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
            ICurrentSettings<ApplicationSettings> applicationSettingsProvider,
            IIniSettingsAssistant iniSettingsAssistant,
            ITranslationUpdater translationUpdater) 
            : base(openFileInteractionHelper, commandLocator, profilesProvider, applicationSettingsProvider, iniSettingsAssistant, translationUpdater)
        {
            _iniSettingsLoader = iniSettingsLoader;
        }

        protected override (IList<ConversionProfile>, IList<PrinterMapping>) LoadFromIniFile(string iniFile)
        {
            if (_iniSettingsLoader.LoadIniSettings(iniFile) is PdfCreatorSettings settings)
            {
                return (settings.ConversionProfiles, settings.ApplicationSettings.PrinterMappings);
            }
            return (new List<ConversionProfile>(), new List<PrinterMapping>());
        }

        public override string Title => Translation.LoadProfilesTitle;
        public override string Description => Translation.LoadSpecificProfiles;
    }
}
