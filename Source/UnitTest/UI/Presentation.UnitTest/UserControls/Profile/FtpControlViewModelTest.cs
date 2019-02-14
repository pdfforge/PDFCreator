using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.FTP;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Translatable;

namespace Presentation.UnitTest.UserControls.Profile
{
    [TestFixture]
    public class FtpControlViewModelTest
    {
        private FtpActionViewModel _viewModel;
        private ObservableCollection<FtpAccount> _ftpAccounts;
        private ICommand _addCommand;
        private ICommand _editCommand;
        private ICurrentSettings<Accounts> _accountsProvider;

        [Test]
        public void DesignTimeViewModelIsNewable()
        {
            var dtvm = new DesignTimeFtpActionViewModel();
            Assert.NotNull(dtvm);
        }

        [SetUp]
        public void SetUp()
        {
            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
            var settingsProvider = Substitute.For<ICurrentSettingsProvider>();
            settingsProvider.SelectedProfile.Returns(new ConversionProfile());

            var settings = new PdfCreatorSettings();
            _ftpAccounts = new ObservableCollection<FtpAccount>();
            settings.ApplicationSettings.Accounts.FtpAccounts = _ftpAccounts;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            var commandLocator = Substitute.For<ICommandLocator>();
            commandLocator.CreateMacroCommand().Returns(x => new MacroCommandBuilder(commandLocator));

            _addCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<FtpAccountAddCommand>().Returns(_addCommand);

            _editCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<FtpAccountEditCommand>().Returns(_editCommand);

            _viewModel = new FtpActionViewModel(translationUpdater, _accountsProvider, settingsProvider, commandLocator, new TokenViewModelFactory(settingsProvider, new TokenHelper(new DesignTimeTranslationUpdater())), null, new GpoSettingsDefaults());
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
            _ftpAccounts.Add(new FtpAccount());

            var latestAccount = new FtpAccount();
            _addCommand.Execute(Arg.Do<object>(o => _ftpAccounts.Add(latestAccount)));

            _viewModel.AddAccountCommand.Execute(null);

            Assert.AreSame(latestAccount, _viewModel.FtpAccountsView.CurrentItem, "Latest Account is not selected Item");
        }

        [Test]
        public void AddAccount_AccountsListisSorted()
        {
            var account1 = new FtpAccount() { UserName = "1" };
            var account2 = new FtpAccount() { UserName = "2" };
            var account3 = new FtpAccount() { UserName = "3" };
            _ftpAccounts.Add(account3);
            _ftpAccounts.Add(account1);
            _addCommand.Execute(Arg.Do<object>(o => _ftpAccounts.Add(account2)));

            _viewModel.AddAccountCommand.Execute(null);

            _viewModel.FtpAccountsView.MoveCurrentToPosition(0);
            Assert.AreSame(account1, _viewModel.FtpAccountsView.CurrentItem);
            _viewModel.FtpAccountsView.MoveCurrentToPosition(1);
            Assert.AreSame(account2, _viewModel.FtpAccountsView.CurrentItem);
            _viewModel.FtpAccountsView.MoveCurrentToPosition(2);
            Assert.AreSame(account3, _viewModel.FtpAccountsView.CurrentItem);
        }

        [Test]
        public void EditAccount_EditedAccountRemainsCurrentItemInView()
        {
            _ftpAccounts.Add(new FtpAccount());
            var editAccount = new FtpAccount();
            _ftpAccounts.Add(editAccount);
            _viewModel.FtpAccountsView.MoveCurrentTo(editAccount);
            _editCommand.Execute(Arg.Do<object>(o => editAccount.UserName = "Edit"));

            _viewModel.EditAccountCommand.Execute(null);

            Assert.AreSame(editAccount, _viewModel.FtpAccountsView.CurrentItem, "Latest Account is not selected Item");
        }

        [Test]
        public void EditAccount_AccountsViewIsSorted()
        {
            var account1 = new FtpAccount() { UserName = "1" };
            var editAccount = new FtpAccount() { UserName = "0" }; //for editing
            var account3 = new FtpAccount() { UserName = "3" };
            _ftpAccounts.Add(account3);
            _ftpAccounts.Add(editAccount);
            _ftpAccounts.Add(account1);
            _editCommand.Execute(Arg.Do<object>(o =>
            {
                var editAccountAsParameter = o as FtpAccount;
                editAccountAsParameter.UserName = "2";
            }));

            _viewModel.EditAccountCommand.Execute(editAccount);

            _viewModel.FtpAccountsView.MoveCurrentToPosition(0);
            Assert.AreSame(account1, _viewModel.FtpAccountsView.CurrentItem);
            _viewModel.FtpAccountsView.MoveCurrentToPosition(1);
            Assert.AreSame(editAccount, _viewModel.FtpAccountsView.CurrentItem);
            _viewModel.FtpAccountsView.MoveCurrentToPosition(2);
            Assert.AreSame(account3, _viewModel.FtpAccountsView.CurrentItem);
        }

        [Test]
        public void DirectoryTokenViewModel_SetActionWritesToCurrentProfilesFtpDirecotry()
        {
            _viewModel.DirectoryTokenViewModel.Text = "Text from TokenViewModel";

            Assert.AreEqual("Text from TokenViewModel", _viewModel.CurrentProfile.Ftp.Directory);
        }

        [Test]
        public void TokenViewModel_GetFunctionReadsFromCurrentProfilesFtpDirecotry()
        {
            _viewModel.CurrentProfile.Ftp.Directory = "Directory from Profile";

            Assert.AreEqual("Directory from Profile", _viewModel.DirectoryTokenViewModel.Text);
        }
    }
}
