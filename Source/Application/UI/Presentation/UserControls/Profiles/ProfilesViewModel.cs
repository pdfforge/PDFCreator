using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor.Commands;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
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
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profileProvider;
        private readonly IRegionManager _regionManager;
        private readonly IWorkflowEditorSubViewProvider _viewProvider;
        private IGpoSettings GpoSettings { get; }

        public ProfilesViewModel(
            ISelectedProfileProvider selectedProfileProvider,
            ITranslationUpdater translationUpdater,
            ICommandLocator commandLocator,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profileProvider,
            IGpoSettings gpoSettings,
            IRegionManager regionManager,
            IWorkflowEditorSubViewProvider viewProvider,
            ICommandBuilderProvider commandBuilderProvider)
            : base(translationUpdater)
        {
            _profileProvider = profileProvider;
            _regionManager = regionManager;
            _viewProvider = viewProvider;

            GpoSettings = gpoSettings;
            SelectedProfileProvider = selectedProfileProvider;

            ProfileRenameCommand = commandLocator.GetCommand<ProfileRenameCommand>();
            ProfileRemoveCommand = commandLocator.GetCommand<ProfileRemoveCommand>();

            var macroCommandBuilder = commandBuilderProvider.ProvideBuilder(commandLocator);

            ProfileAddCommand = macroCommandBuilder
                .AddCommand<ProfileAddCommand>()
                .AddInitializedCommand<WorkflowEditorCommand>(
                    c => c.Initialize(_viewProvider.OutputFormatOverlay, t => t.OutputFormat))
                .AddInitializedCommand<WorkflowEditorCommand>(
                    c => c.Initialize(_viewProvider.SaveOverlay, t => t.Save))
                .Build();
        }

        public void MountView()
        {
            if (SelectedProfileProvider != null)
            {
                SelectedProfileProvider.SettingsChanged += OnSettingsChanged;
                SelectedProfileProvider.SelectedProfileChanged += OnSelectedProfileChanged;
                _profileProvider.Settings.CollectionChanged += OnCollectionChanged;

                _regionManager.RequestNavigate(RegionNames.ProfileLayoutRegion, nameof(WorkflowEditorView));
            }

            foreach (var profile in Profiles)
            {
                profile.MountView();
            }

            OnSettingsChanged(this, null);
        }

        private void OnSelectedProfileChanged(object sender, PropertyChangedEventArgs e)
        {
            var selectedProfile = SelectedProfileProvider.SelectedProfile.Guid;

            if (_selectedProfile != null && selectedProfile == _selectedProfile.ConversionProfile.Guid)
                return;

            _regionManager.RequestNavigate(RegionNames.ProfileLayoutRegion, nameof(WorkflowEditorView));

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
            RaisePropertyChanged(nameof(RenameProfileButtonIsGpoEnabled));
            RaisePropertyChanged(nameof(RemoveProfileButtonIsGpoEnabled));
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

        private ObservableCollection<ConversionProfileWrapper> _profiles;

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

        public bool RenameProfileButtonIsGpoEnabled => !ProfileManagementIsDisabledOrProfileIsShared();
        public bool AddProfileButtonIsGpoEnabled => GpoSettings == null || !LoadSharedProfilesAndDenyUserDefinedProfiles() && !GpoSettings.DisableProfileManagement;
        public bool RemoveProfileButtonIsGpoEnabled => !ProfileManagementIsDisabledOrProfileIsShared();

        private bool ProfileManagementIsDisabledOrProfileIsShared()
        {
            if (GpoSettings != null && GpoSettings.DisableProfileManagement)
                return true;

            if (GpoSettings != null)
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
