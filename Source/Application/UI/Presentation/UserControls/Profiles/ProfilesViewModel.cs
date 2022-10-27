using pdfforge.PDFCreator.Conversion.Jobs;
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
        private readonly IDispatcher _dispatcher;
        private IGpoSettings GpoSettings { get; }

        public ProfilesViewModel(
            ISelectedProfileProvider selectedProfileProvider,
            ITranslationUpdater translationUpdater,
            ICommandLocator commandLocator,
            ICurrentSettings<ObservableCollection<ConversionProfile>> profileProvider,
            IGpoSettings gpoSettings,
            IRegionManager regionManager,
            IWorkflowEditorSubViewProvider viewProvider,
            ICommandBuilderProvider commandBuilderProvider,
            IDispatcher dispatcher)
            : base(translationUpdater)
        {
            _profileProvider = profileProvider;
            _regionManager = regionManager;
            _viewProvider = viewProvider;
            _dispatcher = dispatcher;

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

        private void OnSelectedProfileChanged(object sender, PropertyChangedEventArgs e)
        {
            var selectedProfile = SelectedProfileProvider.SelectedProfile.Guid;

            if (_selectedProfile != null && selectedProfile == _selectedProfile.ConversionProfile.Guid)
                return;

            _regionManager.RequestNavigate(RegionNames.WorkflowEditorView, nameof(WorkflowEditorView));

            _selectedProfile = _profiles.FirstOrDefault(wrapper => wrapper.ConversionProfile.Guid == selectedProfile);

            RaiseChanges();
        }

        private void OnProfileCollectionChanged(object sender, NotifyCollectionChangedEventArgs collectionChangedEventArgs)
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
                    if (_profiles.Any(p => p.ConversionProfile.Guid == conversionProfile.Guid))
                        continue;

                    var profile = new ConversionProfileWrapper(conversionProfile);
                    profile.MountView();
                    _profiles.Add(profile);
                }
            }

            RaiseChanges();
        }

        public void MountView()
        {
            if (SelectedProfileProvider != null)
            {
                SelectedProfileProvider.SettingsChanged += OnSettingsChanged;
                SelectedProfileProvider.SelectedProfileChanged += OnSelectedProfileChanged;

                _regionManager.RequestNavigate(RegionNames.WorkflowEditorView, nameof(WorkflowEditorView));
            }

            if (_profiles == null)
                UpdateProfileWrapperList();

            foreach (var profile in Profiles)
            {
                profile.MountView();
            }

            OnSettingsChanged(this, null);

            _profileProvider.Settings.CollectionChanged += OnProfileCollectionChanged;
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

            _profileProvider.Settings.CollectionChanged -= OnProfileCollectionChanged;
        }

        private void UpdateProfileWrapperList()
        {
            var profileGuid = "";

            foreach (var wrapper in _profiles)
            {
                wrapper.UnmountView();
            }

            _profiles.Clear();
            var wrappers = _profileProvider?.Settings.Select(x => new ConversionProfileWrapper(x)).ToObservableCollection();
            _profiles.AddRange(wrappers);

            foreach (var wrapper in _profiles)
            {
                wrapper.MountView();
            }

            if (SelectedProfileProvider.SelectedProfile != null)
            {
                profileGuid = SelectedProfileProvider.SelectedProfile.Guid;
            }

            var selectedProfile = Profiles.FirstOrDefault(x => x.ConversionProfile.Guid == profileGuid)
                                  ?? Profiles.FirstOrDefault();

            _selectedProfile = selectedProfile;
            SelectedProfileProvider.SelectedProfile = selectedProfile.ConversionProfile;
        }

        private void OnSettingsChanged(object sender, EventArgs e)
        {
            _dispatcher.BeginInvoke(() =>
            {
                UpdateProfileWrapperList();

                RaiseChanges();
            });
        }

        private void RaiseChanges()
        {
            // Important: SelectedProfile must be raised before Profiles.
            // Otherwise, the UI will update the binding source and overwrite the selected profile.
            RaisePropertyChanged(nameof(SelectedProfile));
            RaisePropertyChanged(nameof(Profiles));

            RaisePropertyChanged(nameof(AddProfileButtonIsGpoEnabled));
            RaisePropertyChanged(nameof(EditProfileIsGpoEnabled));
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

        private readonly ObservableCollection<ConversionProfileWrapper> _profiles = new ObservableCollection<ConversionProfileWrapper>();

        public ObservableCollection<ConversionProfileWrapper> Profiles => _profiles;

        public bool EditProfileIsGpoEnabled => !ProfileManagementIsDisabledOrProfileIsShared();
        public bool RemoveProfileButtonIsGpoEnabled => !ProfileManagementIsDisabledOrProfileIsShared();
        public bool AddProfileButtonIsGpoEnabled => GpoSettings == null || !LoadSharedProfilesAndDenyUserDefinedProfiles() && !GpoSettings.DisableProfileManagement;

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
