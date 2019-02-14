using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.FTP
{
    public class FtpActionViewModel : ProfileUserControlViewModel<FtpActionTranslation>
    {
        public ICollectionView FtpAccountsView { get; }

        private readonly ObservableCollection<FtpAccount> _ftpAccounts;

        public IMacroCommand EditAccountCommand { get; }
        public IMacroCommand AddAccountCommand { get; }

        private readonly IGpoSettings _gpoSettings;
        public bool EditAccountsIsDisabled => !_gpoSettings.DisableAccountsTab;

        public TokenViewModel<ConversionProfile> DirectoryTokenViewModel { get; set; }

        public FtpActionViewModel(ITranslationUpdater translationUpdater,
            ICurrentSettings<Conversion.Settings.Accounts> accountsProvider,
            ICurrentSettingsProvider currentSettingsProvider,
            ICommandLocator commandLocator,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            IGpoSettings gpoSettings)

            : base(translationUpdater, currentSettingsProvider, dispatcher)
        {
            _gpoSettings = gpoSettings;
            DirectoryTokenViewModel = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithSelector(p => p.Ftp.Directory)
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListWithFormatting())
                .Build();

            _ftpAccounts = accountsProvider?.Settings.FtpAccounts;

            if (_ftpAccounts != null)
            {
                FtpAccountsView = new ListCollectionView(_ftpAccounts);
                FtpAccountsView.SortDescriptions.Add(new SortDescription(nameof(FtpAccount.AccountInfo), ListSortDirection.Ascending));
            }

            AddAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<FtpAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()))
                .Build();

            EditAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<FtpAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()))
                .Build();
        }

        private void SelectNewAccountInView()
        {
            var latestAccount = _ftpAccounts.Last();
            FtpAccountsView.MoveCurrentTo(latestAccount);
        }

        private void RefreshAccountsView()
        {
            FtpAccountsView.Refresh();
        }
    }
}
