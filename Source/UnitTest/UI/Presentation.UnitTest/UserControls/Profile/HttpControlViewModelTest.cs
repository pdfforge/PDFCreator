using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.HTTP;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Translatable;

namespace Presentation.UnitTest.UserControls.Profile
{
    [TestFixture]
    public class HttpControlViewModelTest
    {
        private HttpActionViewModel _viewModel;
        private ObservableCollection<HttpAccount> _httpAccounts;
        private ICommand _addCommand;
        private ICommand _editCommand;
        private ICurrentSettings<Accounts> _accountsProvider;

        [Test]
        public void DesignTimeViewModelIsNewable()
        {
            var dtvm = new DesignTimeHttpActionViewModel();
            Assert.NotNull(dtvm);
        }

        [SetUp]
        public void SetUp()
        {
            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
            var settingsProvider = Substitute.For<ICurrentSettingsProvider>();
            settingsProvider.SelectedProfile.Returns(new ConversionProfile());

            var settings = new PdfCreatorSettings();
            _httpAccounts = new ObservableCollection<HttpAccount>();
            settings.ApplicationSettings.Accounts.HttpAccounts = _httpAccounts;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            var commandLocator = Substitute.For<ICommandLocator>();
            commandLocator.CreateMacroCommand().Returns(x => new MacroCommandBuilder(commandLocator));

            _addCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<HttpAccountAddCommand>().Returns(_addCommand);
            _editCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<HttpAccountEditCommand>().Returns(_editCommand);

            _viewModel = new HttpActionViewModel(translationUpdater, _accountsProvider, settingsProvider, commandLocator, null, new GpoSettingsDefaults());
        }

        [Test]
        public void Initialize_AddCommandGetsSetByCommandLocator()
        {
            Assert.AreSame(_addCommand, _viewModel.AddAccountCommand.GetCommand(0));
            Assert.IsNotNull(_viewModel.AddAccountCommand.GetCommand(1), "Missing UpdateView Command");
        }

        [Test]
        public void Initialize_EditCommandGetsSetByCommandLocator()
        {
            Assert.AreSame(_editCommand, _viewModel.EditAccountCommand.GetCommand(0));
            Assert.IsNotNull(_viewModel.EditAccountCommand.GetCommand(1), "Missing UpdateView Command");
        }

        [Test]
        public void AddAccount_LatestAccountIsCurrentItemInView()
        {
            _httpAccounts.Add(new HttpAccount());

            var latestAccount = new HttpAccount();
            _addCommand.Execute(Arg.Do<object>(o => _httpAccounts.Add(latestAccount)));

            _viewModel.AddAccountCommand.Execute(null);

            Assert.AreSame(latestAccount, _viewModel.HttpAccountsView.CurrentItem, "Latest Account is not selected Item");
        }

        [Test]
        public void AddAccount_AccountsListisSorted()
        {
            var account1 = new HttpAccount() { Url = "1" };
            var account2 = new HttpAccount() { Url = "2" };
            var account3 = new HttpAccount() { Url = "3" };
            _httpAccounts.Add(account3);
            _httpAccounts.Add(account1);
            _addCommand.Execute(Arg.Do<object>(o => _httpAccounts.Add(account2)));

            _viewModel.AddAccountCommand.Execute(null);

            _viewModel.HttpAccountsView.MoveCurrentToPosition(0);
            Assert.AreSame(account1, _viewModel.HttpAccountsView.CurrentItem);
            _viewModel.HttpAccountsView.MoveCurrentToPosition(1);
            Assert.AreSame(account2, _viewModel.HttpAccountsView.CurrentItem);
            _viewModel.HttpAccountsView.MoveCurrentToPosition(2);
            Assert.AreSame(account3, _viewModel.HttpAccountsView.CurrentItem);
        }

        [Test]
        public void EditAccount_EditedAccountRemainsCurrentItemInView()
        {
            _httpAccounts.Add(new HttpAccount());
            var editAccount = new HttpAccount();
            _httpAccounts.Add(editAccount);
            _viewModel.HttpAccountsView.MoveCurrentTo(editAccount);
            _editCommand.Execute(Arg.Do<object>(o => editAccount.UserName = "Edit"));

            _viewModel.EditAccountCommand.Execute(null);

            Assert.AreSame(editAccount, _viewModel.HttpAccountsView.CurrentItem, "Latest Account is not selected Item");
        }

        [Test]
        public void EditAccount_AccountsViewIsSorted()
        {
            var account1 = new HttpAccount() { Url = "1" };
            var editAccount = new HttpAccount() { Url = "0" }; //for editing
            var account3 = new HttpAccount() { Url = "3" };
            _httpAccounts.Add(account3);
            _httpAccounts.Add(editAccount);
            _httpAccounts.Add(account1);
            _editCommand.Execute(Arg.Do<object>(o =>
            {
                var editAccountAsParameter = o as HttpAccount;
                editAccountAsParameter.Url = "2";
            }));

            _viewModel.EditAccountCommand.Execute(editAccount);

            _viewModel.HttpAccountsView.MoveCurrentToPosition(0);
            Assert.AreSame(account1, _viewModel.HttpAccountsView.CurrentItem);
            _viewModel.HttpAccountsView.MoveCurrentToPosition(1);
            Assert.AreSame(editAccount, _viewModel.HttpAccountsView.CurrentItem);
            _viewModel.HttpAccountsView.MoveCurrentToPosition(2);
            Assert.AreSame(account3, _viewModel.HttpAccountsView.CurrentItem);
        }
    }
}
