
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;


namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class LoadSpecificProfileViewModel : TranslatableViewModelBase<LoadSpecificProfileTranslation>, IMountable, IInteractionAware
    {
        public ICurrentSettings<ObservableCollection<ConversionProfile>> ProfilesProvider { get; }
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;
        private readonly IIniSettingsLoader _iniSettingsLoader;
        public DelegateCommand ChooseIniFileCommand { get; private set; }
        public DelegateCommand ListProfilesCommand { get; private set; }
        public DelegateCommand AddToSettingsCommand { get; private set; }
        public DelegateCommand SelectedProfileChangedCommand { get; private set; }

        public bool HasSelectedProfile => GetSelectedProfiles().Count > 0;
        private ICommand SaveChangedSettingsCommand { get; }

        private PdfCreatorSettings _settings;
        public IList<ProfileSelection> Profiles { get; set; } = new List<ProfileSelection>();

        public string IniFile { get; set; }

        public LoadSpecificProfileViewModel(
            IOpenFileInteractionHelper openFileInteractionHelper,
            IIniSettingsLoader iniSettingsLoader,
            ICommandLocator commandLocator,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider,
            ITranslationUpdater translationUpdater
            ) : base(translationUpdater)
        {
            ProfilesProvider = profilesProvider;
            _openFileInteractionHelper = openFileInteractionHelper;
            _iniSettingsLoader = iniSettingsLoader;
            ChooseIniFileCommand = new DelegateCommand(ChooseIniFileExecute);
            ListProfilesCommand = new DelegateCommand(LoadIniFileExecute);
            AddToSettingsCommand = new DelegateCommand(AddToSettingsExecute);
            SelectedProfileChangedCommand = new DelegateCommand(o => RaisePropertyChanged(nameof(HasSelectedProfile)));
            SaveChangedSettingsCommand = commandLocator.GetCommand<ISaveChangedSettingsCommand>();
        }

        private IList<ConversionProfile> GetSelectedProfiles()
        {
            if (Profiles == null)
                return new List<ConversionProfile>();

            return Profiles
                .Where(selection => selection.IsChecked)
                .Select(selection => selection.Profile)
                .ToList();
        }

        private void AddToSettingsExecute(object obj)
        {
            if (Profiles == null)
            {
                return;
            }

            // Add here to settings
            var newProfiles = GetSelectedProfiles();
            foreach (var conversionProfile in newProfiles)
            {
                var existingProfile = ProfilesProvider.Settings.FirstOrDefault(p => p.Guid == conversionProfile.Guid);

                if (existingProfile != null)
                    existingProfile.ReplaceWith(conversionProfile);
                else
                    ProfilesProvider.Settings.Add(conversionProfile);

            }

            FinishStep();
        }

        private bool ProfileExists(ConversionProfile profile)
        {
            return ProfilesProvider.Settings.FirstOrDefault(currentProfile => currentProfile.Guid == profile.Guid) != null;
        }

        private void LoadIniFileExecute(object obj)
        {
            _settings = (PdfCreatorSettings) _iniSettingsLoader.LoadIniSettings(IniFile);
            if (_settings != null)
            {
                Profiles = _settings
                    .ConversionProfiles
                    .Select(profile => new ProfileSelection { Profile = profile, IsChecked = !ProfileExists(profile)})
                    .OrderByDescending(p => p.IsChecked)
                    .ToList();
                
                RaisePropertyChanged(nameof(Profiles));
                RaisePropertyChanged(nameof(HasSelectedProfile));
            }
        }

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
                LoadIniFileExecute(obj);
                RaisePropertyChanged(nameof(IniFile));
            });

            
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


        private void FinishStep()
        {
            SaveChangedSettingsCommand.Execute(null);
            FinishInteraction?.Invoke();
        }

        public Action FinishInteraction { get; set; }
        public string Title => Translation.Title;
    }

    public class ProfileSelection
    {
        public bool IsChecked { get; set; } = false;
        public ConversionProfile Profile { get; set; }
    }

    public class DesignTimeLoadSpecificProfileViewModel:LoadSpecificProfileViewModel
    {
        public DesignTimeLoadSpecificProfileViewModel() : base(null, null, new DesignTimeCommandLocator(), null, new DesignTimeTranslationUpdater())
        {
        }
    }
}
