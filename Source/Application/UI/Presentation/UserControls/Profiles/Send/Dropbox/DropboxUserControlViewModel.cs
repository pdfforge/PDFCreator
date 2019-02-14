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

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Dropbox
{
    public class DropboxUserControlViewModel : ProfileUserControlViewModel<DropboxTranslation>
    {
        public TokenViewModel<ConversionProfile> SharedFolderTokenViewModel { get; set; }
        public IMacroCommand AddDropboxAccountCommand { get; set; }
        public ObservableCollection<DropboxAccount> DropboxAccounts { get; set; }

        private readonly IGpoSettings _gpoSettings;
        public bool EditAccountsIsDisabled => !_gpoSettings.DisableAccountsTab;

        public DropboxUserControlViewModel(ITranslationUpdater translationUpdater,
            ICurrentSettings<Conversion.Settings.Accounts> accountsProvider,
            ICurrentSettingsProvider currentSettingsProvider,
            ICommandLocator commandLocator,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            IGpoSettings gpoSettings)
            : base(translationUpdater, currentSettingsProvider, dispatcher)
        {
            _gpoSettings = gpoSettings;
            AddDropboxAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<DropboxAccountAddCommand>()
                .AddCommand(new DelegateCommand(SelectNewAccountInView))
                .Build();

            DropboxAccounts = accountsProvider?.Settings.DropboxAccounts;

            translationUpdater.RegisterAndSetTranslation(tf => SetTokenViewModels(tokenViewModelFactory));
        }

        private void SetTokenViewModels(ITokenViewModelFactory tokenViewModelFactory)
        {
            SharedFolderTokenViewModel = tokenViewModelFactory
                .BuilderWithSelectedProfile()
                .WithSelector(p => p.DropboxSettings.SharedFolder)
                .WithDefaultTokenReplacerPreview(th => th.GetTokenListForDirectory())
                .Build();
        }

        private void SelectNewAccountInView(object obj)
        {
            var latestAccount = DropboxAccounts.Last();
            var collectionView = CollectionViewSource.GetDefaultView(DropboxAccounts);
            collectionView.MoveCurrentTo(latestAccount);
        }

        protected override void OnCurrentProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            base.OnCurrentProfileChanged(sender, propertyChangedEventArgs);

            SharedFolderTokenViewModel.RaiseTextChanged();
        }
    }
}
