using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts
{
    public class AccountsViewModel : TranslatableViewModelBase<AccountsTranslation>, IMountable
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly IDispatcher _dispatcher;
        private readonly IGpoSettings _gpoSettings;
        public CompositeCollection AllAccounts { get; } = new CompositeCollection();

        private Conversion.Settings.Accounts Accounts => _accountsProvider?.Settings;
        private readonly ICurrentSettings<Conversion.Settings.Accounts> _accountsProvider;
        private readonly ICommandLocator _commandLocator;
        private object _selectedAccount;

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
            _currentSettingsProvider = currentSettingsProvider;
            _dispatcher = dispatcher;
            _gpoSettings = gpoSettings;
            _accountsProvider = accountProvider;
            _commandLocator = commandLocator;
            ConflateAllAccounts();

            FtpAccountAddCommand = _commandLocator.GetCommand<FtpAccountAddCommand>();
            SmtpAccountAddCommand = _commandLocator.GetCommand<SmtpAccountAddCommand>();
            HttpAccountAddCommand = _commandLocator.GetCommand<HttpAccountAddCommand>();
            DropboxAccountAddCommand = _commandLocator.GetCommand<DropboxAccountAddCommand>();
            TimeServerAccountAddCommand = _commandLocator.GetCommand<TimeServerAccountAddCommand>();

            RemoveSelectedAccountCommand = new DelegateCommand(OnRemoveSelectedCommand, CanRemoveSelectedCommand);

            EditSelectedAccountCommand = new DelegateCommand(OnEditSelectedCommand, CanEditSelectedCommand);

            AddAccountsToken = new List<AccountToken>
            {
                new AccountToken(SmtpAccountAddCommand, () => Translation.AddSmtpAccount, "EmailIcon"),
                new AccountToken(DropboxAccountAddCommand,  () =>Translation.AddDropboxAccount, "DropboxIcon"),
                new AccountToken(FtpAccountAddCommand,  () =>Translation.AddFtpAccount, "FtpIcon"),
                new AccountToken(HttpAccountAddCommand, () => Translation.AddHttpAccount, "HttpIcon"),
                new AccountToken(TimeServerAccountAddCommand, () => Translation.AddTimeServerAccount, "TimeServerIcon")
            };
        }

        private void OnEditSelectedCommand(object obj)
        {
            switch (SelectedAccount)
            {
                case HttpAccount httpAccount:
                    _commandLocator.GetCommand<HttpAccountEditCommand>().Execute(httpAccount);
                    break;

                case SmtpAccount smtpAccount:
                    _commandLocator.GetCommand<SmtpAccountEditCommand>().Execute(smtpAccount);
                    break;

                case FtpAccount ftpAccount:
                    _commandLocator.GetCommand<FtpAccountEditCommand>().Execute(ftpAccount);
                    break;

                case TimeServerAccount timeServerAccount:
                    _commandLocator.GetCommand<TimeServerAccountEditCommand>().Execute(timeServerAccount);
                    break;
            }
        }

        private bool CanEditSelectedCommand(object obj)
        {
            return SelectedAccount != null && !(SelectedAccount is DropboxAccount);
        }

        private bool CanRemoveSelectedCommand(object obj)
        {
            return SelectedAccount != null;
        }

        private void OnRemoveSelectedCommand(object obj)
        {
            switch (SelectedAccount)
            {
                case HttpAccount httpAccount:
                    _commandLocator.GetCommand<HttpAccountRemoveCommand>().Execute(httpAccount);
                    break;

                case SmtpAccount smtpAccount:
                    _commandLocator.GetCommand<SmtpAccountRemoveCommand>().Execute(smtpAccount);
                    break;

                case FtpAccount ftpAccount:
                    _commandLocator.GetCommand<FtpAccountRemoveCommand>().Execute(ftpAccount);
                    break;

                case DropboxAccount dropboxAccount:
                    _commandLocator.GetCommand<DropboxAccountRemoveCommand>().Execute(dropboxAccount);
                    break;

                case TimeServerAccount timeServerAccount:
                    _commandLocator.GetCommand<TimeServerAccountRemoveCommand>().Execute(timeServerAccount);
                    break;
            }
        }

        public List<AccountToken> AddAccountsToken { get; set; }

        public void MountView()
        {
            if (_currentSettingsProvider != null)
            {
                _currentSettingsProvider.SelectedProfileChanged += CurrentSettingsProviderOnSelectedProfileChanged;
                _currentSettingsProvider.SettingsChanged += CurrentSettingsProviderOnSettingsChanged;
            }
        }

        public void UnmountView()
        {
            if (_currentSettingsProvider != null)
            {
                _currentSettingsProvider.SelectedProfileChanged -= CurrentSettingsProviderOnSelectedProfileChanged;
                _currentSettingsProvider.SettingsChanged -= CurrentSettingsProviderOnSettingsChanged;
            }
        }

        protected override void OnTranslationChanged()
        {
            base.OnTranslationChanged();
            RaisePropertyChanged(nameof(AddAccountsToken));
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

        public ICommand SmtpAccountAddCommand { get; }

        public ICommand HttpAccountAddCommand { get; }

        public ICommand DropboxAccountAddCommand { get; }

        public ICommand TimeServerAccountAddCommand { get; }

        public ICommand RemoveSelectedAccountCommand { get; set; }

        public ICommand EditSelectedAccountCommand { get; set; }

        public Object SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                RaisePropertyChanged();
                (RemoveSelectedAccountCommand as DelegateCommand)?.RaiseCanExecuteChanged();
                (EditSelectedAccountCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public class AccountToken
    {
        public ICommand Command { get; set; }
        public string Translation => _getTranslation();
        private readonly Func<string> _getTranslation;
        public string NameOfIcon { get; set; }

        public AccountToken(ICommand command, Func<string> translation, string nameOfIcon)
        {
            Command = command;
            _getTranslation = translation;
            NameOfIcon = nameOfIcon;
        }
    }
}
