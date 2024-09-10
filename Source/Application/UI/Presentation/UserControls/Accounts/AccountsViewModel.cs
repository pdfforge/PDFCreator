using MahApps.Metro.IconPacks;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.Styles.Icons;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
        private readonly EditionHelper _editionHelper;

        public Visibility ShowAddAccountsHint
        {
            get
            {
                var numberOfAccounts = 0;
                if (Accounts != null)
                {
                    numberOfAccounts += Accounts.SmtpAccounts.Count;
                    numberOfAccounts += Accounts.DropboxAccounts.Count;
                    numberOfAccounts += Accounts.MicrosoftAccounts.Count;
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
            IGpoSettings gpoSettings,
            EditionHelper editionHelper)
            : base(translationUpdater)
        {
            _currentSettingsProvider = currentSettingsProvider;
            _dispatcher = dispatcher;
            _gpoSettings = gpoSettings;
            _accountsProvider = accountProvider;
            _commandLocator = commandLocator;
            _editionHelper = editionHelper;
            ConflateAllAccounts();

            FtpAccountAddCommand = _commandLocator.GetCommand<FtpAccountAddCommand>();
            FtpAccountEditCommand = _commandLocator.GetCommand<FtpAccountEditCommand>();
            FtpAccountRemoveCommand = _commandLocator.GetCommand<FtpAccountRemoveCommand>();

            SmtpAccountAddCommand = _commandLocator.GetCommand<SmtpAccountAddCommand>();
            SmtpAccountEditCommand = _commandLocator.GetCommand<SmtpAccountEditCommand>();
            SmtpAccountRemoveCommand = _commandLocator.GetCommand<SmtpAccountRemoveCommand>();

            HttpAccountAddCommand = _commandLocator.GetCommand<HttpAccountAddCommand>();
            HttpAccountEditCommand = _commandLocator.GetCommand<HttpAccountEditCommand>();
            HttpAccountRemoveCommand = _commandLocator.GetCommand<HttpAccountRemoveCommand>();

            DropboxAccountAddCommand = _commandLocator.GetCommand<DropboxAccountAddCommand>();
            DropboxAccountRemoveCommand = _commandLocator.GetCommand<DropboxAccountRemoveCommand>();

            MicrosoftAccountEditCommand = _commandLocator.GetCommand<MicrosoftAccountEditCommand>();
            MicrosoftAccountRemoveCommand = _commandLocator.GetCommand<MicrosoftAccountRemoveCommand>();

            TimeServerAccountAddCommand = _commandLocator.GetCommand<TimeServerAccountAddCommand>();
            TimeServerAccountEditCommand = _commandLocator.GetCommand<TimeServerAccountEditCommand>();
            TimeServerAccountRemoveCommand = _commandLocator.GetCommand<TimeServerAccountRemoveCommand>();

            UpdateMenuItemList();
        }

        public void UpdateMenuItemList()
        {
            AddAccountMenuItems.Clear();

            AddAccountMenuItems.Add(new MenuItem
            {
                Header = Translation.AddSmtpAccount,
                Command = SmtpAccountAddCommand,
                Icon = new PackIconMaterialDesign { Kind = PackIconMaterialDesignKind.Mail, Width = 25 }
            });
            if (!_editionHelper.IsServer)
            {
                if (_accountsProvider?.Settings?.MicrosoftAccounts?.Count == 0)
                {
                    AddAccountMenuItems.Add(new MenuItem
                    {
                        Header = Translation.AddMicrosoftAccount,
                        Command = MicrosoftAccountEditCommand,
                        Icon = new PackIconMaterialDesign { Kind = PackIconMaterialDesignKind.Mail, Width = 25 }
                    });
                }
            }

            AddAccountMenuItems.Add(new MenuItem
            {
                Header = Translation.AddDropboxAccount,
                Command = DropboxAccountAddCommand,
                Icon = IconResource.TryFindResource("DropboxIcon")
            });

            AddAccountMenuItems.Add(new MenuItem
            {
                Header = Translation.AddFtpAccount,
                Command = FtpAccountAddCommand,
                Icon = IconResource.TryFindResource("FtpIcon")
            });

            AddAccountMenuItems.Add(new MenuItem
            {
                Header = Translation.AddHttpAccount,
                Command = HttpAccountAddCommand,
                Icon = new PackIconMaterialDesign { Kind = PackIconMaterialDesignKind.Http, Width = 23 }
            });

            AddAccountMenuItems.Add(new MenuItem
            {
                Header = Translation.AddTimeServerAccount,
                Command = TimeServerAccountAddCommand,
                Icon = IconResource.TryFindResource("TimeServerIcon")
            });

            RaisePropertyChanged(nameof(AddAccountMenuItems));
        }

        public List<MenuItem> AddAccountMenuItems { get; private set; } = new List<MenuItem>();

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
            RaisePropertyChanged(nameof(AddAccountMenuItems));
            if (_editionHelper != null)
                UpdateMenuItemList();
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

            Accounts.MicrosoftAccounts.CollectionChanged += RaiseAddAccountsBelowVisibilityChanged;
            AllAccounts.Add(new CollectionContainer { Collection = Accounts.MicrosoftAccounts });

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
            UpdateMenuItemList();
        }

        public ICommand FtpAccountAddCommand { get; }
        public ICommand FtpAccountEditCommand { get; }
        public ICommand FtpAccountRemoveCommand { get; }

        public ICommand MicrosoftAccountEditCommand { get; }
        public ICommand MicrosoftAccountRemoveCommand { get; }

        public ICommand SmtpAccountAddCommand { get; }
        public ICommand SmtpAccountEditCommand { get; }
        public ICommand SmtpAccountRemoveCommand { get; }

        public ICommand HttpAccountAddCommand { get; }
        public ICommand HttpAccountEditCommand { get; }
        public ICommand HttpAccountRemoveCommand { get; }

        public ICommand DropboxAccountAddCommand { get; }
        public ICommand DropboxAccountRemoveCommand { get; }

        public ICommand TimeServerAccountAddCommand { get; }
        public ICommand TimeServerAccountEditCommand { get; }
        public ICommand TimeServerAccountRemoveCommand { get; }
    }
}