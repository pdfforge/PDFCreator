using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts
{
    public class AccountsViewModel : TranslatableViewModelBase<AccountsTranslation>, IMountable
    {
        private readonly IDispatcher _dispatcher;
        private readonly IGpoSettings _gpoSettings;
        public CompositeCollection AllAccounts { get; } = new CompositeCollection();

        private Conversion.Settings.Accounts Accounts => _accountsProvider?.Settings;
        private readonly ICurrentSettings<Conversion.Settings.Accounts> _accountsProvider;
        private readonly ICommandLocator _commandLocator;

        public Visibility ShowAddAccountsHint
        {
            get
            {
                var numberOfAccounts = 0;
                if (Accounts != null)
                {
                    numberOfAccounts += Accounts.SmtpAccounts.Count;
                    numberOfAccounts += Accounts.DropboxAccounts.Count;
                    numberOfAccounts += Accounts.FtpAccounts.Count;
                    numberOfAccounts += Accounts.HttpAccounts.Count;
                    numberOfAccounts += Accounts.TimeServerAccounts.Count;
                }

                return numberOfAccounts <= 4 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public AccountsViewModel(
            ICurrentSettingsProvider currentSettingsProvider,
            ICurrentSettings<Conversion.Settings.Accounts> accountProvider,
            ICommandLocator commandLocator,
            ITranslationUpdater translationUpdater,
            IDispatcher dispatcher,
            IGpoSettings gpoSettings)
            : base(translationUpdater)
        {
            _dispatcher = dispatcher;
            _gpoSettings = gpoSettings;
            _accountsProvider = accountProvider;
            _commandLocator = commandLocator;
            ConflateAllAccounts();

            FtpAccountAddCommand = commandLocator.GetCommand<FtpAccountAddCommand>();
            FtpAccountRemoveCommand = commandLocator.GetCommand<FtpAccountRemoveCommand>();
            FtpAccountEditCommand = _commandLocator.GetCommand<FtpAccountEditCommand>();

            SmtpAccountAddCommand = commandLocator.GetCommand<SmtpAccountAddCommand>();
            SmtpAccountRemoveCommand = commandLocator.GetCommand<SmtpAccountRemoveCommand>();
            SmtpAccountEditCommand = _commandLocator.GetCommand<SmtpAccountEditCommand>();

            HttpAccountAddCommand = commandLocator.GetCommand<HttpAccountAddCommand>();
            HttpAccountRemoveCommand = commandLocator.GetCommand<HttpAccountRemoveCommand>();
            HttpAccountEditCommand = _commandLocator.GetCommand<HttpAccountEditCommand>();

            DropboxAccountAddCommand = commandLocator.GetCommand<DropboxAccountAddCommand>();
            DropboxAccountRemoveCommand = commandLocator.GetCommand<DropboxAccountRemoveCommand>();

            TimeServerAccountAddCommand = commandLocator.GetCommand<TimeServerAccountAddCommand>();
            TimeServerAccountRemoveCommand = commandLocator.GetCommand<TimeServerAccountRemoveCommand>();
            TimeServerAccountEditCommand = _commandLocator.GetCommand<TimeServerAccountEditCommand>();

            if (currentSettingsProvider != null)
            {
                currentSettingsProvider.SelectedProfileChanged += CurrentSettingsProviderOnSelectedProfileChanged;
                currentSettingsProvider.SettingsChanged += CurrentSettingsProviderOnSettingsChanged;
            }
        }

        public void MountView()
        {
            ((IMountable)FtpAccountEditCommand).MountView();
            ((IMountable)SmtpAccountEditCommand).MountView();
            ((IMountable)HttpAccountEditCommand).MountView();
            ((IMountable)TimeServerAccountEditCommand).MountView();
        }

        public void UnmountView()
        {
            ((IMountable)FtpAccountEditCommand).UnmountView();
            ((IMountable)SmtpAccountEditCommand).UnmountView();
            ((IMountable)HttpAccountEditCommand).UnmountView();
            ((IMountable)TimeServerAccountEditCommand).UnmountView();
        }

        private void CurrentSettingsProviderOnSettingsChanged(object sender, EventArgs eventArgs)
        {
            _dispatcher.BeginInvoke(ConflateAllAccounts);
        }

        private void CurrentSettingsProviderOnSelectedProfileChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            _dispatcher.BeginInvoke(ConflateAllAccounts);
        }

        private void ConflateAllAccounts()
        {
            if (Accounts == null)
                return;

            AllAccounts.Clear();

            Accounts.SmtpAccounts.CollectionChanged += RaiseAddAccountsBelowVisibilityChanged;
            AllAccounts.Add(new CollectionContainer { Collection = Accounts.SmtpAccounts });

            Accounts.DropboxAccounts.CollectionChanged += RaiseAddAccountsBelowVisibilityChanged;
            AllAccounts.Add(new CollectionContainer { Collection = Accounts.DropboxAccounts });

            Accounts.FtpAccounts.CollectionChanged += RaiseAddAccountsBelowVisibilityChanged;
            AllAccounts.Add(new CollectionContainer { Collection = Accounts.FtpAccounts });

            Accounts.HttpAccounts.CollectionChanged += RaiseAddAccountsBelowVisibilityChanged;
            AllAccounts.Add(new CollectionContainer { Collection = Accounts.HttpAccounts });

            Accounts.TimeServerAccounts.CollectionChanged += RaiseAddAccountsBelowVisibilityChanged;
            AllAccounts.Add(new CollectionContainer { Collection = Accounts.TimeServerAccounts });

            RaisePropertyChanged(nameof(AllAccounts));
        }

        public object IsAccountsDisabled
        {
            get
            {
                if (Accounts == null)
                    return false;

                return _gpoSettings != null ? _gpoSettings.DisableAccountsTab : false;
            }
        }

        private void RaiseAddAccountsBelowVisibilityChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            RaisePropertyChanged(nameof(ShowAddAccountsHint));
        }

        public ICommand FtpAccountAddCommand { get; }
        public ICommand FtpAccountRemoveCommand { get; }
        public ICommand FtpAccountEditCommand { get; set; }

        public ICommand SmtpAccountAddCommand { get; }
        public ICommand SmtpAccountRemoveCommand { get; }
        public ICommand SmtpAccountEditCommand { get; set; }

        public ICommand HttpAccountAddCommand { get; }
        public ICommand HttpAccountRemoveCommand { get; }
        public ICommand HttpAccountEditCommand { get; set; }

        public ICommand DropboxAccountAddCommand { get; }
        public ICommand DropboxAccountRemoveCommand { get; }

        public ICommand TimeServerAccountAddCommand { get; }
        public ICommand TimeServerAccountRemoveCommand { get; }
        public ICommand TimeServerAccountEditCommand { get; set; }
    }
}
