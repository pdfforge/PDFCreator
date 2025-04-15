using System.Collections.ObjectModel;
using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions.OneDrive;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OneDrive
{
    public class OneDriveActionViewModel : ActionViewModelBase<OneDriveAction, OneDriveTranslation>
    {
        private readonly IGpoSettings _gpoSettings;
        private readonly EditionHelper _editionHelper;

        public OneDriveActionViewModel(IActionLocator actionLocator, ErrorCodeInterpreter errorCodeInterpreter, ITranslationUpdater translationUpdater,
            ICurrentSettingsProvider currentSettingsProvider, IDispatcher dispatcher, IDefaultSettingsBuilder defaultSettingsBuilder, IActionOrderHelper actionOrderHelper,
            ITokenViewModelFactory tokenViewModelFactory, IGpoSettings gpoSettings, ICommandLocator commandLocator, EditionHelper editionHelper) :
            base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
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
                    .WithSelector(p => p.OneDriveSettings.SharedFolder)
                    .WithDefaultTokenReplacerPreview(th => th.GetTokenListForDirectory())
                    .Build();
            });
        }

        public IMacroCommand AddMicrosoftAccountCommand { get; set; }
        public IMacroCommand EditMicrosoftAccountCommand { get; set; }

        private void SelectNewAccountInView()
        {
            var accountToBeSelected = MicrosoftAccounts.LastOrDefault();
            var collectionView = CollectionViewSource.GetDefaultView(MicrosoftAccounts);
            collectionView.MoveCurrentTo(accountToBeSelected);
        }

        public bool IsNotServer => !_editionHelper.IsServer;
        public bool EditAccountsIsDisabled => _gpoSettings != null && _gpoSettings.DisableAccountsTab;

        public ObservableCollection<MicrosoftAccount> MicrosoftAccounts { get; set; }

        public TokenViewModel<ConversionProfile> SharedFolderTokenViewModel { get; set; }

        public bool CreateShareLink
        {
            get => CurrentProfile != null && CurrentProfile.OneDriveSettings.CreateShareLink;
            set
            {
                if (value == CurrentProfile.OneDriveSettings.CreateShareLink) 
                    return;

                CurrentProfile.OneDriveSettings.CreateShareLink = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowLink
        {
            get => CurrentProfile != null && CurrentProfile.OneDriveSettings.ShowShareLink;
            set
            {
                if (value == CurrentProfile.OneDriveSettings.ShowShareLink) return;
 
                CurrentProfile.OneDriveSettings.ShowShareLink = value;
                RaisePropertyChanged();
            }
        }

        public bool OpenUploadedFile
        {
            get => CurrentProfile != null && CurrentProfile.OneDriveSettings.OpenUploadedFile;
            set
            {
                if (value == CurrentProfile.OneDriveSettings.OpenUploadedFile) return;

                CurrentProfile.OneDriveSettings.OpenUploadedFile = value;
                RaisePropertyChanged();
            }
        }

        public bool EnsureUniqueOneDriveFilenames
        {
            get => CurrentProfile != null && CurrentProfile.OneDriveSettings.EnsureUniqueFilenames;
            set
            {
                if (value == CurrentProfile.OneDriveSettings.EnsureUniqueFilenames) return;
                CurrentProfile.OneDriveSettings.EnsureUniqueFilenames = value;
                RaisePropertyChanged();
            }
        }

        protected override string SettingsPreviewString
        {
            get
            {
                var microsoftAccount = Accounts.GetOneDriveAccount(CurrentProfile);
                return microsoftAccount != null ? microsoftAccount.AccountInfo : string.Empty;
            }
        }

        public override void MountView()
        {
            SharedFolderTokenViewModel.MountView();
            
            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            SharedFolderTokenViewModel.UnmountView();
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);

            SharedFolderTokenViewModel.RaiseTextChanged();
        }
    }
}
