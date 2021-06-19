using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
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
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.HTTP
{
    public class HttpActionViewModel : ActionViewModelBase<HttpAction, HttpTranslation>
    {
        public ICollectionView HttpAccountsView { get; }

        private ObservableCollection<HttpAccount> _httpAccounts;
        public IMacroCommand EditAccountCommand { get; }
        public IMacroCommand AddAccountCommand { get; }

        private readonly IGpoSettings _gpoSettings;
        public bool EditAccountsIsDisabled => !_gpoSettings.DisableAccountsTab;

        public HttpActionViewModel(ITranslationUpdater translationUpdater,
            IActionLocator actionLocator, ErrorCodeInterpreter errorCodeInterpreter,
            ICurrentSettingsProvider currentSettingsProvider,
            ICommandLocator commandLocator,
            IDispatcher dispatcher,
            IGpoSettings gpoSettings,
            IDefaultSettingsBuilder defaultSettingsBuilder,
            IActionOrderHelper actionOrderHelper)
            : base(actionLocator, errorCodeInterpreter, translationUpdater, currentSettingsProvider, dispatcher, defaultSettingsBuilder, actionOrderHelper)
        {
            _gpoSettings = gpoSettings;
            _httpAccounts = currentSettingsProvider.CheckSettings.Accounts.HttpAccounts;

            HttpAccountsView = new ListCollectionView(_httpAccounts);
            HttpAccountsView.SortDescriptions.Add(new SortDescription(nameof(HttpAccount.AccountInfo), ListSortDirection.Ascending));
            HttpAccountsView.CurrentChanged += (sender, args) => RaisePropertyChanged(nameof(ShowAutosaveRequiresPasswords));

            AddAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<HttpAccountAddCommand>()
                .AddCommand(new DelegateCommand(o => SelectNewAccountInView()))
                .Build();

            EditAccountCommand = commandLocator.CreateMacroCommand()
                .AddCommand<HttpAccountEditCommand>()
                .AddCommand(new DelegateCommand(o => RefreshAccountsView()))
                .AddCommand(new DelegateCommand(o => StatusChanged()))
                .Build();
        }

        private void SelectNewAccountInView()
        {
            var latestAccount = _httpAccounts.Last();
            HttpAccountsView.MoveCurrentTo(latestAccount);
        }

        private void RefreshAccountsView()
        {
            HttpAccountsView.Refresh();
        }

        public bool ShowAutosaveRequiresPasswords
        {
            get
            {
                if (CurrentProfile == null)
                    return false;

                if (!CurrentProfile.AutoSave.Enabled)
                    return false;

                if (!(HttpAccountsView.CurrentItem is HttpAccount currentAccount))
                    return false;

                if (!currentAccount.IsBasicAuthentication)
                    return false;

                return string.IsNullOrWhiteSpace(currentAccount.Password);
            }
        }

        protected override string SettingsPreviewString
        {
            get
            {
                var httpAccount = Accounts.HttpAccounts.FirstOrDefault(x => x.AccountId == CurrentProfile.HttpSettings.AccountId);
                return httpAccount != null ? httpAccount.Url : string.Empty;
            }
        }

        public override void MountView()
        {
            EditAccountCommand.MountView();
            base.MountView();
        }

        public override void UnmountView()
        {
            base.UnmountView();
            EditAccountCommand.UnmountView();
        }
    }
}
