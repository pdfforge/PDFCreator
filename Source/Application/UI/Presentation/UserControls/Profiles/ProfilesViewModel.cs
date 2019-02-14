using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class ProfilesViewModel : TranslatableViewModelBase<ProfileMangementTranslation>, IMountable
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profileProvider;
        private IGpoSettings GpoSettings { get; }

        public ProfilesViewModel(ISelectedProfileProvider selectedProfileProvider, ITranslationUpdater translationUpdater, ICommandLocator commandLocator, ISettingsProvider settingsProvider, ICurrentSettings<ObservableCollection<ConversionProfile>> profileProvider, IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            _settingsProvider = settingsProvider;
            _profileProvider = profileProvider;
            GpoSettings = gpoSettings;
            SelectedProfileProvider = selectedProfileProvider;

            ProfileAddCommand = commandLocator.GetCommand<ProfileAddCommand>();
            ProfileRenameCommand = commandLocator.GetCommand<ProfileRenameCommand>();
            ProfileRemoveCommand = commandLocator.GetCommand<ProfileRemoveCommand>();
        }

        public void MountView()
        {
            if (SelectedProfileProvider != null)
            {
                SelectedProfileProvider.SettingsChanged += OnProfilesChanged;
                SelectedProfileProvider.SelectedProfileChanged += RaiseChanges;
            }
            OnProfilesChanged(this, null);
            RaiseChanges(this, null);
        }

        public void UnmountView()
        {
            if (SelectedProfileProvider != null)
            {
                SelectedProfileProvider.SettingsChanged -= OnProfilesChanged;
                SelectedProfileProvider.SelectedProfileChanged -= RaiseChanges;
            }
        }

        private void OnProfilesChanged(object sender, EventArgs e)
        {
            var profileGuid = SelectedProfile.Guid;
            RaisePropertyChanged(nameof(Profiles));

            var selectedProfile = Profiles.FirstOrDefault(x => x.Guid == profileGuid)
                ?? Profiles.FirstOrDefault();

            SelectedProfileProvider.SelectedProfile = selectedProfile;
            RaiseChanges(this, null);
        }

        private void RaiseChanges(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(SelectedProfile));
            RaisePropertyChanged(nameof(EditProfileIsGpoDisabled));
            RaisePropertyChanged(nameof(RenameProfileButtonIsGpoEnabled));
            RaisePropertyChanged(nameof(RemoveProfileButtonIsGpoEnabled));
        }

        public ConversionProfile SelectedProfile
        {
            get { return SelectedProfileProvider?.SelectedProfile; }
            set { SelectedProfileProvider.SelectedProfile = value; }
        }

        public ObservableCollection<ConversionProfile> Profiles
        {
            get { return _profileProvider?.Settings; }
            set { }
        }

        public bool EditProfileIsGpoDisabled => ProfileManagementIsDisabledOrProfileIsShared();

        public bool RenameProfileButtonIsGpoEnabled => !ProfileManagementIsDisabledOrProfileIsShared();
        public bool AddProfileButtonIsGpoEnabled => GpoSettings == null || !LoadSharedProfilesAndDenyUserDefinedProfiles() && !GpoSettings.DisableProfileManagement;
        public bool RemoveProfileButtonIsGpoEnabled => !ProfileManagementIsDisabledOrProfileIsShared();

        private bool ProfileManagementIsDisabledOrProfileIsShared()
        {
            if (GpoSettings != null && GpoSettings.DisableProfileManagement)
                return true;

            if (SelectedProfile != null && SelectedProfile.Properties.IsShared)
                return true;

            return false;
        }

        private bool LoadSharedProfilesAndDenyUserDefinedProfiles()
        {
            if (GpoSettings != null)
            {
                return GpoSettings.LoadSharedProfiles && !GpoSettings.AllowUserDefinedProfiles;
            }

            return false;
        }

        public ISelectedProfileProvider SelectedProfileProvider { get; }

        public ICommand ProfileAddCommand { get; set; }
        public ICommand ProfileRenameCommand { get; set; }
        public ICommand ProfileRemoveCommand { get; set; }
    }
}
