using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Translatable;

namespace Presentation.UnitTest.UserControls.AccountViewModels
{
    [TestFixture]
    public class AccountsViewModelTest
    {
        private AccountsViewModel _viewModel;

        private Accounts _accounts;

        private ICommandLocator _commandLocator;

        private ICommand _ftpAccountAddCommand;
        private ICommand _ftpAccountEditCommand;
        private ICommand _ftpAccountRemoveCommand;

        private ICommand _smtpAccountAddCommand;
        private ICommand _smtpAccountEditCommand;
        private ICommand _smtpAccountRemoveCommand;

        private ICommand _httpAccountAddCommand;
        private ICommand _httpAccountEditCommand;
        private ICommand _httpAccountRemoveCommand;

        private ICommand _dropboxAccountAddCommand;
        private ICommand _dropboxAccountRemoveCommand;

        private ICommand _timeServerAccountAddCommand;
        private ICommand _timeServerAccountEditCommand;
        private ICommand _timeServerAccountRemoveCommand;

        private ICommand _saveApplicationSettingsChangesCommand;

        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _accounts = new Accounts();
            _accounts.FtpAccounts = new ObservableCollection<FtpAccount>();
            _accounts.FtpAccounts.Add(new FtpAccount());
            _accounts.SmtpAccounts = new ObservableCollection<SmtpAccount>();
            _accounts.SmtpAccounts.Add(new SmtpAccount());
            _accounts.HttpAccounts = new ObservableCollection<HttpAccount>();
            _accounts.HttpAccounts.Add(new HttpAccount());
            _accounts.DropboxAccounts = new ObservableCollection<DropboxAccount>();
            _accounts.DropboxAccounts.Add(new DropboxAccount());
            _accounts.TimeServerAccounts = new ObservableCollection<TimeServerAccount>();
            _accounts.TimeServerAccounts.Add(new TimeServerAccount());

            _commandLocator = Substitute.For<ICommandLocator>();
            _commandLocator.CreateMacroCommand().Returns(x => new MacroCommandBuilder(_commandLocator));

            _ftpAccountAddCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<FtpAccountAddCommand>().Returns(_ftpAccountAddCommand);

            _ftpAccountEditCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<FtpAccountEditCommand>().Returns(_ftpAccountEditCommand);

            _ftpAccountRemoveCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<FtpAccountRemoveCommand>().Returns(_ftpAccountRemoveCommand);

            _smtpAccountAddCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<SmtpAccountAddCommand>().Returns(_smtpAccountAddCommand);

            _smtpAccountEditCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<SmtpAccountEditCommand>().Returns(_smtpAccountEditCommand);

            _smtpAccountRemoveCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<SmtpAccountRemoveCommand>().Returns(_smtpAccountRemoveCommand);

            _httpAccountAddCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<HttpAccountAddCommand>().Returns(_httpAccountAddCommand);

            _httpAccountEditCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<HttpAccountEditCommand>().Returns(_httpAccountEditCommand);

            _httpAccountRemoveCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<HttpAccountRemoveCommand>().Returns(_httpAccountRemoveCommand);

            _dropboxAccountAddCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<DropboxAccountAddCommand>().Returns(_dropboxAccountAddCommand);

            _dropboxAccountRemoveCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<DropboxAccountRemoveCommand>().Returns(_dropboxAccountRemoveCommand);

            _timeServerAccountAddCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<TimeServerAccountAddCommand>().Returns(_timeServerAccountAddCommand);

            _timeServerAccountEditCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<TimeServerAccountEditCommand>().Returns(_timeServerAccountEditCommand);

            _timeServerAccountRemoveCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<TimeServerAccountRemoveCommand>().Returns(_timeServerAccountRemoveCommand);

            _saveApplicationSettingsChangesCommand = Substitute.For<IMountableCommand>();
            _commandLocator.GetCommand<ISaveChangedSettingsCommand>().Returns(_saveApplicationSettingsChangesCommand);

            InitViewModel();
        }

        private void InitViewModel()
        {
            var settingsProvider = Substitute.For<ICurrentSettingsProvider>();
            var settings = Substitute.For<ICurrentSettings<Accounts>>();

            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(_accounts);
            settings.Settings.Returns(_accounts);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _viewModel = new AccountsViewModel(settingsProvider, _accountsProvider, _commandLocator, translationUpdater, new InvokeImmediatelyDispatcher(), null);
            _viewModel.MountView();
        }

        [Test]
        public void ShowAddAccountsHint_MoreThan4Accounts_IsDisabled()
        {
            //In the Setup Method more than 5 Accounts are added.

            Assert.AreEqual(Visibility.Collapsed, _viewModel.ShowAddAccountsHint);
        }

        [Test]
        public void ShowAddAccountsHint_4Accounts_IsEnabled()
        {
            _accounts = new Accounts(); //clear
            _accounts.SmtpAccounts = new ObservableCollection<SmtpAccount>();
            for (int i = 0; i < 4; i++)
                _accounts.SmtpAccounts = new ObservableCollection<SmtpAccount>();

            InitViewModel();

            Assert.AreEqual(Visibility.Visible, _viewModel.ShowAddAccountsHint);
        }

        #region FTP Accounts

        [Test]
        public void Initialize_AllAccountsContainsFtpAccounts()
        {
            foreach (CollectionContainer collectionContainer in _viewModel.AllAccounts)
            {
                var collection = collectionContainer.Collection as ObservableCollection<FtpAccount>;
                if (collection == null)
                    continue;

                Assert.AreSame(_accounts.FtpAccounts, collection);
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void AddFtpAccount_TriggersRaisePropertyChangedOfShowAddAccountsBelow()
        {
            var wasCalled = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(_viewModel.ShowAddAccountsHint)))
                    wasCalled = true;
            };

            _accounts.FtpAccounts.Add(new FtpAccount());

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Initialize_FtpAccountAddCommandIsSet()
        {
            Assert.AreSame(_ftpAccountAddCommand, _viewModel.FtpAccountAddCommand);
        }

        [Test]
        public void Initialize_FtpAccountEditCommandIsSet()
        {
            Assert.AreSame(_ftpAccountEditCommand, _viewModel.FtpAccountEditCommand);
        }

        [Test]
        public void Initialize_FtpAccountRemoveCommandIsSet()
        {
            Assert.AreSame(_ftpAccountRemoveCommand, _viewModel.FtpAccountRemoveCommand);
        }

        #endregion FTP Accounts

        #region SMTP Accounts

        [Test]
        public void Initialize_AllAccountsContainsSmtpAccounts()
        {
            foreach (CollectionContainer collectionContainer in _viewModel.AllAccounts)
            {
                var collection = collectionContainer.Collection as ObservableCollection<SmtpAccount>;
                if (collection == null)
                    continue;

                Assert.AreSame(_accounts.SmtpAccounts, collection);
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void AddSmtpAccount_TriggersRaisePropertyChangedOfShowAddAccountsBelow()
        {
            var wasCalled = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(_viewModel.ShowAddAccountsHint)))
                    wasCalled = true;
            };

            _accounts.SmtpAccounts.Add(new SmtpAccount());

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Initialize_SmtpAccountAddCommandIsSet()
        {
            Assert.AreSame(_smtpAccountAddCommand, _viewModel.SmtpAccountAddCommand);
        }

        [Test]
        public void Initialize_SmtpAccountEditCommandIsSet()
        {
            Assert.AreSame(_smtpAccountEditCommand, _viewModel.SmtpAccountEditCommand);
        }

        [Test]
        public void Initialize_SmtpAccountRemoveCommandIsSet()
        {
            Assert.AreSame(_smtpAccountRemoveCommand, _viewModel.SmtpAccountRemoveCommand);
        }

        #endregion SMTP Accounts

        #region HTTP Accounts

        [Test]
        public void Initialize_AllAccountsContainHttpAccounts()
        {
            foreach (CollectionContainer collectionContainer in _viewModel.AllAccounts)
            {
                var collection = collectionContainer.Collection as ObservableCollection<HttpAccount>;
                if (collection == null)
                    continue;

                Assert.AreSame(_accounts.HttpAccounts, collection);
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void AddHttpAccount_TriggersRaisePropertyChangedOfShowAddAccountsBelow()
        {
            var wasCalled = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(_viewModel.ShowAddAccountsHint)))
                    wasCalled = true;
            };

            _accounts.HttpAccounts.Add(new HttpAccount());

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Initialize_HttpAccountAddCommandIsSet()
        {
            Assert.AreSame(_httpAccountAddCommand, _viewModel.HttpAccountAddCommand);
        }

        [Test]
        public void Initialize_HttpAccountEditCommandIsSet()
        {
            Assert.AreSame(_httpAccountEditCommand, _viewModel.HttpAccountEditCommand);
        }

        [Test]
        public void Initialize_HttpAccountRemoveCommandIsSet()
        {
            Assert.AreSame(_httpAccountRemoveCommand, _viewModel.HttpAccountRemoveCommand);
        }

        #endregion HTTP Accounts

        #region Dropbox Accounts

        [Test]
        public void Initialize_AllAccountsContainDropboxAccounts()
        {
            foreach (CollectionContainer collectionContainer in _viewModel.AllAccounts)
            {
                var collection = collectionContainer.Collection as ObservableCollection<DropboxAccount>;
                if (collection == null)
                    continue;

                Assert.AreSame(_accounts.DropboxAccounts, collection);
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void AddDropboxAccount_TriggersRaisePropertyChangedOfShowAddAccountsBelow()
        {
            var wasCalled = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(_viewModel.ShowAddAccountsHint)))
                    wasCalled = true;
            };

            _accounts.DropboxAccounts.Add(new DropboxAccount());

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Initialize_DropboxAccountAddCommandIsSet()
        {
            Assert.AreSame(_dropboxAccountAddCommand, _viewModel.DropboxAccountAddCommand);
        }

        [Test]
        public void Initialize_DropboxAccountRemoveCommandIsSet()
        {
            Assert.AreSame(_dropboxAccountRemoveCommand, _viewModel.DropboxAccountRemoveCommand);
        }

        #endregion Dropbox Accounts

        #region TimeServer Accounts

        [Test]
        public void Initialize_AllAccountsContainTimeServerAccounts()
        {
            foreach (CollectionContainer collectionContainer in _viewModel.AllAccounts)
            {
                var collection = collectionContainer.Collection as ObservableCollection<TimeServerAccount>;
                if (collection == null)
                    continue;

                Assert.AreSame(_accounts.TimeServerAccounts, collection);
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void AddTimeServerAccount_TriggersRaisePropertyChangedOfShowAddAccountsBelow()
        {
            var wasCalled = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName.Equals(nameof(_viewModel.ShowAddAccountsHint)))
                    wasCalled = true;
            };

            _accounts.TimeServerAccounts.Add(new TimeServerAccount());

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Initialize_TimeServerAccountAddCommandIsSet()
        {
            Assert.AreSame(_timeServerAccountAddCommand, _viewModel.TimeServerAccountAddCommand);
        }

        [Test]
        public void Initialize_TimeServerAccountEditCommandIsSet()
        {
            Assert.AreSame(_timeServerAccountEditCommand, _viewModel.TimeServerAccountEditCommand);
        }

        [Test]
        public void Initialize_TimeServerAccountRemoveCommandIsSet()
        {
            Assert.AreSame(_timeServerAccountRemoveCommand, _viewModel.TimeServerAccountRemoveCommand);
        }

        #endregion TimeServer Accounts

        [Test]
        public void DesignTimeViewModel_IsNewable()
        {
            var dtvm = new DesignTimeAccountsViewModel();
            Assert.IsNotNull(dtvm);
        }
    }
}
