using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.SettingsManagement;
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

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Dropbox
{
    public class DropboxUserControlViewModel : ActionViewModelBase<DropboxAction, DropboxTranslation>
    {
        public TokenViewModel<ConversionProfile> SharedFolderTokenViewModel { get; set; }
        public IMacroCommand AddDropboxAccountCommand { get; set; }
        public ObservableCollection<DropboxAccount> DropboxAccounts { get; set; }

        private readonly ITranslationUpdater _translationUpdater;
        private readonly ITokenViewModelFactory _tokenViewModelFactory;
        private readonly IGpoSettings _gpoSettings;
        public bool EditAccountsIsDisabled => !_gpoSettings.DisableAccountsTab;

        public DropboxUserControlViewModel(
            IActionLocator actionLocator,
            ErrorCodeInterpreter errorCodeInterpreter,
            ITranslationUpdater translationUpdater,
            ICurrentSettingsProvider currentSettingsProvider,
            ICommandLocator commandLocator,
            ITokenViewModelFactory tokenViewModelFactory,
            IDispatcher dispatcher,
            IGpoSettings gpoSettings,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _translationUpdater = translationUpdater;
            _tokenViewModelFactory = tokenViewModelFactory;
            _gpoSettings = gpoSettings;
            AddDropboxAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<DropboxAccountAddCommand>()
                .AddCommand(new DelegateCommand(SelectNewAccountInView))
                .Build();

            DropboxAccounts = currentSettingsProvider.CheckSettings.Accounts.DropboxAccounts;

            _translationUpdater.RegisterAndSetTranslation(tf =>
            {
                SharedFolderTokenViewModel = _tokenViewModelFactory
                    .BuilderWithSelectedProfile()
                    .WithSelector(p => p.DropboxSettings.SharedFolder)
                    .WithDefaultTokenReplacerPreview(th => th.GetTokenListForDirectory())
                    .Build();
            });
        }

        protected override string SettingsPreviewString
        {
            get
            {
                var dropBoxAccount = Accounts.DropboxAccounts.FirstOrDefault(x => x.AccountId == CurrentProfile.DropboxSettings.AccountId);
                return dropBoxAccount != null ? dropBoxAccount.AccountInfo : string.Empty;
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
