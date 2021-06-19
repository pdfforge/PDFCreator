using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement.DefaultSettings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Ftp;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.FTP
{
    public class FtpActionViewModel : ActionViewModelBase<FtpAction, FtpActionTranslation>
    {
        public ICollectionView FtpAccountsView { get; private set; }

        private ObservableCollection<FtpAccount> _ftpAccounts;

        public IMacroCommand EditAccountCommand { get; private set; }
        public IMacroCommand AddAccountCommand { get; private set; }

        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly IGpoSettings _gpoSettings;
        public bool EditAccountsIsDisabled => !_gpoSettings.DisableAccountsTab;

        public TokenViewModel<ConversionProfile> DirectoryTokenViewModel { get; set; }

        public FtpActionViewModel(ITranslationUpdater translationUpdater,
            ICurrentSettingsProvider currentSettingsProvider,
            ICommandLocator commandLocator,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            IGpoSettings gpoSettings,
            IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            var commandLocator1 = commandLocator;
            _tokenViewModelFactory = tokenViewModelFactory;
            _gpoSettings = gpoSettings;

            _ftpAccounts = currentSettingsProvider.CheckSettings.Accounts.FtpAccounts;

            FtpAccountsView = new ListCollectionView(_ftpAccounts);
            FtpAccountsView.SortDescriptions.Add(new SortDescription(nameof(FtpAccount.AccountInfo), ListSortDirection.Ascending));
            FtpAccountsView.CurrentChanged += (sender, args) => RaisePropertyChanged(nameof(ShowAutosaveRequiresPasswords));

            AddAccountCommand = commandLocator1.CreateMacroCommand()
                .AddCommand<FtpAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()))
                .Build();

            EditAccountCommand = commandLocator1.CreateMacroCommand()
                .AddCommand<FtpAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()))
                .AddCommand(new DelegateCommand(o => StatusChanged()))
                .Build();

            DirectoryTokenViewModel = _tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithSelector(p => p.Ftp.Directory)
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListWithFormatting())
                .Build();
        }

        protected override string SettingsPreviewString
        {
            get
            {
                var ftpAccount = Accounts.FtpAccounts.FirstOrDefault(x => x.AccountId == CurrentProfile?.Ftp?.AccountId);
                return ftpAccount != null ? Translation.FormatFtpConnectionName(ftpAccount.Server, ftpAccount.FtpConnectionType) : string.Empty;
            }
        }

        public override void MountView()
        {
            DirectoryTokenViewModel.MountView();
            EditAccountCommand.MountView();
            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            DirectoryTokenViewModel.UnmountView();
            EditAccountCommand.UnmountView();
        }

        private void SelectNewAccountInView()
        {
            var latestAccount = _ftpAccounts.Last();
            FtpAccountsView.MoveCurrentTo(latestAccount);
        }

        public bool ShowAutosaveRequiresPasswords
        {
            get
            {
                if (CurrentProfile == null)
                    return false;

                if (!CurrentProfile.AutoSave.Enabled)
                    return false;

                if (!(FtpAccountsView.CurrentItem is FtpAccount currentAccount))
                    return false;

                if (currentAccount.AuthenticationType == AuthenticationType.KeyFileAuthentication
                    && !currentAccount.KeyFileRequiresPass)
                    return false;

                return string.IsNullOrWhiteSpace(currentAccount.Password);
            }
        }

        private void RefreshAccountsView()
        {
            FtpAccountsView.Refresh();
        }
    }
}
