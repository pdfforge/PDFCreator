using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Events;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class ProfilesViewModel : TranslatableViewModelBase<ProfileMangementTranslation>, IMountable
    {
        private readonly ISettingsProvider _settingsProvider;
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profileProvider;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private IGpoSettings GpoSettings { get; }

        public ProfilesViewModel(
            ISelectedProfileProvider selectedProfileProvider,
            ITranslationUpdater translationUpdater,
            ICommandLocator commandLocator,
            ISettingsProvider settingsProvider,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profileProvider,
            IGpoSettings gpoSettings,
            IRegionManager regionManager,
            IEventAggregator eventAggregator)
            : base(translationUpdater)
        {
            _settingsProvider = settingsProvider;
            _profileProvider = profileProvider;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            GpoSettings = gpoSettings;
            SelectedProfileProvider = selectedProfileProvider;

            ProfileAddCommand = commandLocator.GetCommand<ProfileAddCommand>();
            ProfileRenameCommand = commandLocator.GetCommand<ProfileRenameCommand>();
            ProfileRemoveCommand = commandLocator.GetCommand<ProfileRemoveCommand>();
            _switchLayoutCommand = commandLocator.GetCommand<SwitchLayoutCommand>() as IAsyncCommand;
        }

        public void MountView()
        {
            if (SelectedProfileProvider != null)
            {
                SelectedProfileProvider.SettingsChanged += OnSettingsChanged;
                SelectedProfileProvider.SelectedProfileChanged += OnSelectedProfileChanged;
                _profileProvider.Settings.CollectionChanged += OnCollectionChanged;
                _settingsProvider.Settings.ApplicationSettings.PropertyChanged += (sender, args) => RaisePropertyChanged(nameof(IsWorkflowEditorEnabled));

                if (SelectedProfileProvider.SelectedProfile.EnableWorkflowEditor)
                {
                    _regionManager.RequestNavigate(RegionNames.ProfileLayoutRegion, nameof(WorkflowEditorView));
                }
                else
                {
                    _regionManager.RequestNavigate(RegionNames.ProfileLayoutRegion, nameof(TabBasedProfileLayoutView));
                }

                _eventAggregator.GetEvent<SwitchWorkflowLayoutEvent>().Subscribe(RaiseChanges);

                RaisePropertyChanged(nameof(IsWorkflowEditorEnabled));
            }

            foreach (var profile in Profiles)
            {
                profile.MountView();
            }

            OnSettingsChanged(this, null);
        }

        public bool IsWorkflowEditorEnabled
        {
            get
            {
                if (GpoSettings != null && GpoSettings.DisableProfileManagement)
                    return false;

                if (SelectedProfileProvider?.SelectedProfile == null)
                    return false;

                return SelectedProfileProvider.SelectedProfile.EnableWorkflowEditor;
            }
            set
            {
                if (SelectedProfileProvider.SelectedProfile.EnableWorkflowEditor == value)
                    return;

                OnLayoutSwitchTriggered(value);
            }
        }

        private void OnLayoutSwitchTriggered(object parameter)
        {
            if (_switchLayoutCommand.CanExecute(parameter))
                _switchLayoutCommand.ExecuteAsync(parameter);
        }

        private void OnSelectedProfileChanged(object sender, PropertyChangedEventArgs e)
        {
            var selectedProfile = SelectedProfileProvider.SelectedProfile.Guid;

            if (_selectedProfile != null && selectedProfile == _selectedProfile.ConversionProfile.Guid)
                return;

            if (SelectedProfileProvider.SelectedProfile.EnableWorkflowEditor)
            {
                _regionManager.RequestNavigate(RegionNames.ProfileLayoutRegion, nameof(WorkflowEditorView));
            }
            else
            {
                _regionManager.RequestNavigate(RegionNames.ProfileLayoutRegion, nameof(TabBasedProfileLayoutView));
            }

            _selectedProfile = _profiles.FirstOrDefault(wrapper => wrapper.ConversionProfile.Guid == selectedProfile);

            RaiseChanges();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs collectionChangedEventArgs)
        {
            if (collectionChangedEventArgs == null)
                return;

            if (collectionChangedEventArgs.OldItems != null)
            {
                var toDelete = new List<ConversionProfileWrapper>();
                foreach (ConversionProfile eOldItem in collectionChangedEventArgs.OldItems)
                {
                    var conversionProfileWrapper = _profiles.FirstOrDefault(wrapper => wrapper.ConversionProfile.Guid == eOldItem?.Guid);
                    if (conversionProfileWrapper != null)
                        toDelete.Add(conversionProfileWrapper);
                }
                toDelete.ForEach(wrapper =>
                {
                    _profiles.Remove(wrapper);
                    wrapper.UnmountView();
                });
            }

            if (collectionChangedEventArgs.NewItems != null)
            {
                foreach (ConversionProfile conversionProfile in collectionChangedEventArgs.NewItems)
                {
                    var profile = new ConversionProfileWrapper(conversionProfile);
                    profile.MountView();
                    _profiles.Add(profile);
                }
            }

            RaiseChanges();
        }

        public void UnmountView()
        {
            if (SelectedProfileProvider != null)
            {
                SelectedProfileProvider.SelectedProfileChanged -= OnSelectedProfileChanged;
                SelectedProfileProvider.SettingsChanged -= OnSettingsChanged;
            }

            foreach (var profile in Profiles)
            {
                profile.UnmountView();
            }
            _eventAggregator.GetEvent<SwitchWorkflowLayoutEvent>().Unsubscribe(RaiseChanges);
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            var profileGuid = "";

            _profiles = _profileProvider?.Settings.Select(x => new ConversionProfileWrapper(x)).ToObservableCollection();

            if (SelectedProfileProvider.SelectedProfile != null)
            {
                profileGuid = SelectedProfileProvider.SelectedProfile.Guid;
            }

            var selectedProfile = Profiles.FirstOrDefault(x => x.ConversionProfile.Guid == profileGuid)
                                  ?? Profiles.FirstOrDefault();

            _selectedProfile = selectedProfile;
            SelectedProfileProvider.SelectedProfile = selectedProfile.ConversionProfile;

            RaiseChanges();
        }

        private void RaiseChanges()
        {
            // Important: SelectedProfile must be raised before Profiles.
            // Otherwise, the UI will update the binding source and overwrite the selected profile.
            RaisePropertyChanged(nameof(SelectedProfile));
            RaisePropertyChanged(nameof(Profiles));

            RaisePropertyChanged(nameof(EditProfileIsGpoDisabled));
            RaisePropertyChanged(nameof(WorkflowEditorToggleButtonIsGpoEnabled));
            RaisePropertyChanged(nameof(RenameProfileButtonIsGpoEnabled));
            RaisePropertyChanged(nameof(RemoveProfileButtonIsGpoEnabled));
            RaisePropertyChanged(nameof(IsWorkflowEditorEnabled));
        }

        private ConversionProfileWrapper _selectedProfile = null;

        public ConversionProfileWrapper SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                if (value != null)
                {
                    SelectedProfileProvider.SelectedProfile = value.ConversionProfile;
                    _selectedProfile = value;
                }
            }
        }

        public ApplicationSettings AppSettings => _settingsProvider?.Settings?.ApplicationSettings;

        private ObservableCollection<ConversionProfileWrapper> _profiles;
        private IAsyncCommand _switchLayoutCommand;

        public ObservableCollection<ConversionProfileWrapper> Profiles
        {
            get
            {
                if (_profiles == null)
                {
                    _profiles = _profileProvider?.Settings.Select(x => new ConversionProfileWrapper(x)).ToObservableCollection();
                }
                return _profiles;
            }
            set { /* the property has to have a setter because of wpf binding */}
        }

        public bool EditProfileIsGpoDisabled => ProfileManagementIsDisabledOrProfileIsShared();

        public bool WorkflowEditorToggleButtonIsGpoEnabled => !ProfileManagementIsDisabledOrProfileIsShared();
        public bool RenameProfileButtonIsGpoEnabled => !ProfileManagementIsDisabledOrProfileIsShared();
        public bool AddProfileButtonIsGpoEnabled => GpoSettings == null || !LoadSharedProfilesAndDenyUserDefinedProfiles() && !GpoSettings.DisableProfileManagement;
        public bool RemoveProfileButtonIsGpoEnabled => !ProfileManagementIsDisabledOrProfileIsShared();

        private bool ProfileManagementIsDisabledOrProfileIsShared()
        {
            if (GpoSettings != null && GpoSettings.DisableProfileManagement)
                return true;

            if (GpoSettings != null && !GpoSettings.AllowSharedProfilesEditing)
            {
                if (SelectedProfile != null && SelectedProfile.ConversionProfile.Properties.IsShared)
                    return true;
            }

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
