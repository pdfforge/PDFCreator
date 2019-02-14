using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailSmtp;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Threading;
using pdfforge.PDFCreator.Utilities.Tokens;
using Ploeh.AutoFixture;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Translatable;

namespace Presentation.UnitTest.UserControls.Profile
{
    [TestFixture]
    public class SmtpControlViewModelTest
    {
        private SmtpActionViewModel _viewModel;
        private ConversionProfile _profile;
        private ObservableCollection<SmtpAccount> _smtpAccounts;
        private ICommand _addCommand;
        private ICommand _editCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ISmtpTest _smtpTest;
        private ITokenHelper _tokenHelper;
        private TokenReplacer _tokenReplacer;
        private ICurrentSettings<Accounts> _accountsProvider;

        private readonly IFixture _fixture = new Fixture();

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();

            _smtpTest = Substitute.For<ISmtpTest>();

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _profile = new ConversionProfile();
            var currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            currentSettingsProvider.SelectedProfile.Returns(_profile);

            var settings = new PdfCreatorSettings();
            _smtpAccounts = new ObservableCollection<SmtpAccount>();
            settings.ApplicationSettings.Accounts.SmtpAccounts = _smtpAccounts;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            var commandLocator = Substitute.For<ICommandLocator>();
            commandLocator.CreateMacroCommand().Returns(x => new MacroCommandBuilder(commandLocator));

            _addCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<SmtpAccountAddCommand>().Returns(_addCommand);
            _editCommand = Substitute.For<ICommand>();
            commandLocator.GetCommand<SmtpAccountEditCommand>().Returns(_editCommand);

            _tokenHelper = new TokenHelper(new DesignTimeTranslationUpdater());
            _tokenReplacer = _tokenHelper.TokenReplacerWithPlaceHolders;

            _viewModel = new SmtpActionViewModel(_interactionRequest, _smtpTest, translationUpdater, currentSettingsProvider, _accountsProvider, commandLocator, new TokenViewModelFactory(currentSettingsProvider, new TokenHelper(new DesignTimeTranslationUpdater())), null, new GpoSettingsDefaults());
        }

        [Test]
        public void DesignTimeViewModelIsNewable()
        {
            var dtvm = new DesignTimeSmtpActionViewModel();
            Assert.NotNull(dtvm);
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
        public void RecipientsTokenViewModel_UsesRecipientsFromProfile()
        {
            var expectedString = _fixture.Create<string>();
            _profile.EmailSmtpSettings.Recipients = expectedString;
            Assert.AreEqual(expectedString, _viewModel.RecipientsTokenViewModel.Text);
        }

        [Test]
        public void RecipientsTokenViewModel_UpdatesRecipientsInProfile()
        {
            var expectedString = _fixture.Create<string>();
            _viewModel.RecipientsTokenViewModel.Text = expectedString;
            Assert.AreEqual(expectedString, _profile.EmailSmtpSettings.Recipients);
        }

        [Test]
        public void RecipientsTokenViewModel_TokensAreTokenListForEmail()
        {
            var tokenList = _tokenHelper.GetTokenListForEmailRecipients();

            foreach (var tokenWithCommand in _viewModel.RecipientsTokenViewModel.Tokens)
                tokenList.Remove(tokenWithCommand.Name);

            Assert.IsEmpty(tokenList);
        }

        [Test]
        public void RecipientsTokenViewModel_Preview_IsTextWithReplacedTokens()
        {
            var tokenName = _tokenHelper.GetTokenListForEmailRecipients()[0];
            var tokenValue = _tokenReplacer.ReplaceTokens(tokenName);

            _viewModel.RecipientsTokenViewModel.Text = tokenName;

            Assert.AreEqual(tokenValue, _viewModel.RecipientsTokenViewModel.Preview);
        }

        [Test]
        public void AddAccount_LatestAccountIsCurrentItemInView()
        {
            _smtpAccounts.Add(new SmtpAccount());

            var latestAccount = new SmtpAccount();
            _addCommand.Execute(Arg.Do<object>(o => _smtpAccounts.Add(latestAccount)));

            _viewModel.AddAccountCommand.Execute(null);

            Assert.AreSame(latestAccount, _viewModel.SmtpAccountsView.CurrentItem, "Latest Account is not selected Item");
        }

        [Test]
        public void AddAccount_AccountsListisSorted()
        {
            var account1 = new SmtpAccount() { Address = "1" };
            var account2 = new SmtpAccount() { Address = "2" };
            var account3 = new SmtpAccount() { Address = "3" };
            _smtpAccounts.Add(account3);
            _smtpAccounts.Add(account1);
            _addCommand.Execute(Arg.Do<object>(o => _smtpAccounts.Add(account2)));

            _viewModel.AddAccountCommand.Execute(null);

            _viewModel.SmtpAccountsView.MoveCurrentToPosition(0);
            Assert.AreSame(account1, _viewModel.SmtpAccountsView.CurrentItem);
            _viewModel.SmtpAccountsView.MoveCurrentToPosition(1);
            Assert.AreSame(account2, _viewModel.SmtpAccountsView.CurrentItem);
            _viewModel.SmtpAccountsView.MoveCurrentToPosition(2);
            Assert.AreSame(account3, _viewModel.SmtpAccountsView.CurrentItem);
        }

        [Test]
        public void EditAccount_EditedAccountRemainsCurrentItemInView()
        {
            _smtpAccounts.Add(new SmtpAccount());
            var editAccount = new SmtpAccount();
            _smtpAccounts.Add(editAccount);
            _viewModel.SmtpAccountsView.MoveCurrentTo(editAccount);
            _editCommand.Execute(Arg.Do<object>(o => editAccount.UserName = "Edit"));

            _viewModel.EditAccountCommand.Execute(null);

            Assert.AreSame(editAccount, _viewModel.SmtpAccountsView.CurrentItem, "Latest Account is not selected Item");
        }

        [Test]
        public void EditAccount_AccountsViewIsSorted()
        {
            var account1 = new SmtpAccount() { Address = "1" };
            var editAccount = new SmtpAccount() { Address = "0" }; //for editing
            var account3 = new SmtpAccount() { Address = "3" };
            _smtpAccounts.Add(account3);
            _smtpAccounts.Add(editAccount);
            _smtpAccounts.Add(account1);
            _editCommand.Execute(Arg.Do<object>(o =>
            {
                var editAccountAsParameter = o as SmtpAccount;
                editAccountAsParameter.Address = "2";
            }));

            _viewModel.EditAccountCommand.Execute(editAccount);

            _viewModel.SmtpAccountsView.MoveCurrentToPosition(0);
            Assert.AreSame(account1, _viewModel.SmtpAccountsView.CurrentItem);
            _viewModel.SmtpAccountsView.MoveCurrentToPosition(1);
            Assert.AreSame(editAccount, _viewModel.SmtpAccountsView.CurrentItem);
            _viewModel.SmtpAccountsView.MoveCurrentToPosition(2);
            Assert.AreSame(account3, _viewModel.SmtpAccountsView.CurrentItem);
        }
    }
}
