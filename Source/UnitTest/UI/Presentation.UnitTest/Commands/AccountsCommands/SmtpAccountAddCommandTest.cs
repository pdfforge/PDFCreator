using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.ObjectModel;
using System.Linq;
using Translatable;

namespace Presentation.UnitTest.Commands.AccountsCommands
{
    [TestFixture]
    public class SmtpAccountAddCommandTest
    {
        private IWaitableCommand _smtpAccountAddCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ObservableCollection<SmtpAccount> _smtpAccounts;
        private readonly AccountsTranslation _translation = new AccountsTranslation();
        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();

            _smtpAccounts = new ObservableCollection<SmtpAccount>();
            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.SmtpAccounts = _smtpAccounts;
            var currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _smtpAccountAddCommand = new SmtpAccountAddCommand(_interactionRequest, _accountsProvider, translationUpdater);
        }

        [Test]
        public void CanExecute_AccountsListIsNull_DoesNotThrowException()
        {
            _smtpAccounts = null;
            Assert.DoesNotThrow(() => _smtpAccountAddCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_IsAlwaysTrue()
        {
            Assert.IsTrue(_smtpAccountAddCommand.CanExecute(null));
        }

        [Test]
        public void AddAccount_InteractionRequestRaisesSmtpAccountInteraction()
        {
            _smtpAccountAddCommand.Execute(null);

            _interactionRequest.AssertWasRaised<SmtpAccountInteraction>();
        }

        [Test]
        public void AddAccount_RaisedInteractionHasCorrectTitle()
        {
            _smtpAccountAddCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<SmtpAccountInteraction>();

            Assert.AreEqual(_translation.AddSmtpAccount, interaction.Title, "Wrong Title for SmtpAccountInteraction");
        }

        [Test]
        public void AddAccount_RaisedInteractionContainsAccountWithID()
        {
            _smtpAccountAddCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<SmtpAccountInteraction>();
            Assert.IsFalse(string.IsNullOrWhiteSpace(interaction.SmtpAccount.AccountId), "Raised interaction contains account without ID");
        }

        [Test]
        public void AddAccount_UserCancelsInteraction_NoAccountGetsAdded()
        {
            _interactionRequest.RegisterInteractionHandler<SmtpAccountInteraction>(i =>
            {
                i.Success = false; // User cancels
            });

            _smtpAccountAddCommand.Execute(null);

            Assert.IsEmpty(_smtpAccounts);
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_AccountGetsAdded()
        {
            var newAccount = new SmtpAccount();
            newAccount.Address = "New Address";
            newAccount.Password = "New Password";
            newAccount.Server = "New Server";

            _interactionRequest.RegisterInteractionHandler<SmtpAccountInteraction>(i =>
            {
                i.Success = true;
                i.SmtpAccount = newAccount;
            });

            _smtpAccountAddCommand.Execute(null);

            Assert.AreEqual(1, _smtpAccounts.Count);
            Assert.AreSame(newAccount, _smtpAccounts.FirstOrDefault());
        }

        [Test]
        public void AddAccount_UserCancelsInteraction_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _smtpAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;

            _interactionRequest.RegisterInteractionHandler<SmtpAccountInteraction>(i =>
            {
                i.Success = false; // User cancels
            });

            _smtpAccountAddCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _smtpAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;

            _interactionRequest.RegisterInteractionHandler<SmtpAccountInteraction>(i =>
            {
                i.Success = true; // User applies
            });

            _smtpAccountAddCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }
    }
}
