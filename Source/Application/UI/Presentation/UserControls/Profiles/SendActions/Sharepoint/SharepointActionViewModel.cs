using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Sharepoint;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Sharepoint;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Sharepoint
{
    public class SharepointActionViewModel : ActionViewModelBase<SharepointAction, SharepointTranslation>
    {

        private readonly SharepointHelper _sharepointHelper;
        private readonly ICurrentSettings<Conversion.Settings.Accounts> _microsoftAccounts;
        private readonly IDispatcher _dispatchingThread;
        private readonly IGpoSettings _gpoSettings;
        private readonly EditionHelper _editionHelper;
        public IMacroCommand AddMicrosoftAccountCommand { get; set; }
        public IMacroCommand EditMicrosoftAccountCommand { get; set; }

        public MicrosoftAccount SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                if (value == null )
                    return;

                if (_selectedAccount != null && _selectedAccount.AccountId == value?.AccountId)
                    return;

                
                _selectedAccount = value;
                CurrentProfile.SharepointSettings.AccountId = _selectedAccount.AccountId;

                if (_sharepointHelper != null)
                {
                    Task.Run(async () => await UpdateSites(value));
                }

                StatusChanged();
            }
        }


        public ObservableCollection<MicrosoftAccount> MicrosoftAccounts { get; set; }

        private ObservableCollection<SharepointDrive> _drives;

        public ObservableCollection<SharepointDrive> Drives
        {
            get => _drives;
            set => _drives = value;
        }

        public SharepointDrive SelectedDrive
        {
            get => _selectedDrive;
            set
            {

                if (value == null)
                {
                    return;
                }

                CurrentProfile.SharepointSettings.DriveId = value.Id;
                _selectedDrive = value;
            }
        }

        public bool ShowLink
        {
            get => CurrentProfile != null && CurrentProfile.SharepointSettings.ShowShareLink;
            set
            {
                if (value == CurrentProfile.SharepointSettings.ShowShareLink) return;

                CurrentProfile.SharepointSettings.ShowShareLink = value;
                RaisePropertyChanged();
            }
        }

        public bool OpenUploadedFile
        {
            get => CurrentProfile != null && CurrentProfile.SharepointSettings.OpenUploadedFile;
            set
            {
                if (value == CurrentProfile.SharepointSettings.OpenUploadedFile) return;

                CurrentProfile.SharepointSettings.OpenUploadedFile = value;
                RaisePropertyChanged();
            }
        }

        public bool EnsureUniqueFilenames
        {
            get => CurrentProfile != null && CurrentProfile.SharepointSettings.EnsureUniqueFilenames;
            set
            {
                if (value == CurrentProfile.SharepointSettings.EnsureUniqueFilenames) return;
                CurrentProfile.SharepointSettings.EnsureUniqueFilenames = value;
                RaisePropertyChanged();
            }
        }


        private ObservableCollection<SharepointSite> _sites;

        private SharepointSite _selectedSite;
        private SharepointDrive _selectedDrive;
        private MicrosoftAccount _selectedAccount;


        public SharepointSite SelectedSite
        {
            set
            {
                if(value == null) 
                    return;

                CurrentProfile.SharepointSettings.SiteId = value.Id;
                _selectedSite = value;
                RaisePropertyChanged();

                if (_sharepointHelper != null)
                {
                   Task.Run(async () => await UpdateDrives(value) );
                }
            }
            get
            {
                return _selectedSite;
            }
        }

        public bool IsLoadingAccounts { get; set; } = false;
        public bool IsLoadingSites { get; set; } = true;
        public bool IsLoadingDrives { get; set; } = true;

        private async Task UpdateSites(MicrosoftAccount _)
        {
            IsLoadingSites = true;
            SelectedSite = null;
            RaisePropertyChanged(nameof(IsLoadingSites));
            RaisePropertyChanged(nameof(SelectedSite));

            var sites = await _sharepointHelper.GetSiteFor(MsAccount());
            if (sites == null)
                return;



            _dispatchingThread.BeginInvoke(() =>
            {
                sites.Sort((site1, site2) => string.Compare(site1.DisplayName, site2.DisplayName, StringComparison.Ordinal));
                Sites = new ObservableCollection<SharepointSite>(sites);
                IsLoadingSites = false;
                RaisePropertyChanged(nameof(IsLoadingSites));

                var preselectedSite = _sites.FirstOrDefault(site => site.Id == CurrentProfile.SharepointSettings.SiteId);
                SelectedSite = preselectedSite ?? _sites.FirstOrDefault();

                IsLoadingSites = false;
                RaisePropertyChanged(nameof(Sites));
                RaisePropertyChanged(nameof(SelectedSite));
                StatusChanged();
            });
        }

        private async Task UpdateDrives(SharepointSite value)
        {
            IsLoadingDrives = true;
            SelectedDrive = null;
            RaisePropertyChanged(nameof(IsLoadingDrives));
            RaisePropertyChanged(nameof(SelectedDrive));


            var drives = await _sharepointHelper.GetDrivesForSite(MsAccount(), value);

            _dispatchingThread.BeginInvoke(() =>
            {
                drives.Sort((drive1, drive2) => string.Compare(drive1.Name, drive2.Name, StringComparison.Ordinal));

                _drives = new ObservableCollection<SharepointDrive>(drives);
                
                IsLoadingDrives = false;
                RaisePropertyChanged(nameof(IsLoadingDrives));
                RaisePropertyChanged(nameof(Drives));


                SelectedDrive = _drives.FirstOrDefault();
                RaisePropertyChanged(nameof(SelectedDrive));
                StatusChanged();
            });
        }

        public TokenViewModel<ConversionProfile> SharedFolderTokenViewModel { get; set; }

        public ObservableCollection<SharepointSite> Sites
        {
            get => _sites;
            set
            {
                _sites = value;
                RaisePropertyChanged();
            }
        }

        public bool IsServer => _editionHelper?.IsServer ?? false;

        public SharepointActionViewModel(
            IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            ITranslationUpdater translationUpdater,
            ICurrentSettingsProvider currentSettingsProvider,
            IDispatcher dispatcher,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper,
            SharepointHelper sharepointHelper,
            ICurrentSettings<Conversion.Settings.Accounts> microsoftAccounts, 
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatchingThread,
            IGpoSettings gpoSettings,
            ICommandLocator commandLocator,
            EditionHelper editionHelper) : 
            base(actionLocator,
                errorCodeInterpreter,
                translationUpdater,
                currentSettingsProvider,
                dispatcher,
                defaultSettingsBuilder,
                actionOrderHelper)
        {
            _sharepointHelper = sharepointHelper;
            _microsoftAccounts = microsoftAccounts;
            _dispatchingThread = dispatchingThread;
            _gpoSettings = gpoSettings;
            _editionHelper = editionHelper;

            MicrosoftAccounts = currentSettingsProvider.CheckSettings.Accounts.MicrosoftAccounts;

            AddMicrosoftAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<MicrosoftAccountEditCommand>()
                .AddCommand(new DelegateCommand(_ => RaisePropertyChanged(nameof(MicrosoftAccounts))))
                .AddCommand(new DelegateCommand(_ => SelectNewAccountInView()))
                .AddCommand(new DelegateCommand(_ => StatusChanged()))
                .Build();

            EditMicrosoftAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<MicrosoftAccountEditCommand>()
                .AddCommand(new DelegateCommand(_ => StatusChanged()))
                .Build();


            translationUpdater.RegisterAndSetTranslation(tf =>
            {
                SharedFolderTokenViewModel = tokenViewModelFactory
                    .BuilderWithSelectedProfile()
                    .WithSelector(p => p.SharepointSettings.SharedFolder)
                    .WithDefaultTokenReplacerPreview(th => th.GetTokenListForDirectory())
                    .Build();
            });
        }
        
        private void SelectNewAccountInView()
        {
            var accountToBeSelected = MicrosoftAccounts.LastOrDefault();
            var collectionView = CollectionViewSource.GetDefaultView(MicrosoftAccounts);
            collectionView.MoveCurrentTo(accountToBeSelected);
        }

        private MicrosoftAccount MsAccount()
        {
            return  _microsoftAccounts.Settings.MicrosoftAccounts.FirstOrDefault(account => account.AccountId == CurrentProfile.SharepointSettings.AccountId);
        }


        public override async void MountView()
        {
            base.MountView();

            var microsoftAccount = MsAccount();
            if (microsoftAccount != null)
            {
                var sites = await _sharepointHelper.GetSiteFor(microsoftAccount);
                var preselectedSite = _sites.FirstOrDefault(site => site.Id == CurrentProfile.SharepointSettings.SiteId);
                SelectedSite = preselectedSite ?? _sites.FirstOrDefault();
                _sites = new ObservableCollection<SharepointSite>(sites);
            }

            

            RaisePropertyChanged(nameof(SelectedSite));
            RaisePropertyChanged(nameof(Sites));
        }
        public bool EditAccountsIsDisabled => _gpoSettings != null && _gpoSettings.DisableAccountsTab;


        protected override string SettingsPreviewString
        {
            get
            {
                var account = _microsoftAccounts.Settings.MicrosoftAccounts.FirstOrDefault(account => account.AccountId == CurrentProfile?.SharepointSettings?.AccountId);
                return $"{account.AccountInfo}";
            }
        }
    }
}
