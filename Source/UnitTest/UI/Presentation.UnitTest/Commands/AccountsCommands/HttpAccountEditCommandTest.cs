using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.ObjectModel;
using Translatable;

namespace Presentation.UnitTest.Commands.AccountsCommands
{
    [TestFixture]
    public class HttpAccountEditCommandTest
    {
        private IWaitableCommand _httpAccountEditCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ObservableCollection<HttpAccount> _httpAccounts;
        private HttpAccount _currentHttpAccount;
        private HttpTranslation _translation;
        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _translation = new HttpTranslation();
            _currentHttpAccount = new HttpAccount() { UserName = "CurrentUserName" };
            _httpAccounts = new ObservableCollection<HttpAccount>();
            _httpAccounts.Add(_currentHttpAccount);
            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.HttpAccounts = _httpAccounts;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
            _httpAccountEditCommand = new HttpAccountEditCommand(_interactionRequest, _accountsProvider, translationUpdater);
        }

        [Test]
        public void CanExecute_AccountsListIsNull_DoesNotThrowException()
        {
            _httpAccounts = null;

            Assert.DoesNotThrow(() => _httpAccountEditCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListIsEmpty_ReturnsFalse()
        {
            _httpAccounts.Clear();

            Assert.IsFalse(_httpAccountEditCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListHasAccount_ReturnsTrue()
        {
            Assert.IsTrue(_httpAccountEditCommand.CanExecute(null));
        }

        [Test]
        public void EditAccount_GivenParameterIsNotHttpAccount_NoInteraction()
        {
            _httpAccountEditCommand.Execute(null);

            _interactionRequest.AssertWasNotRaised<HttpAccountInteraction>();
        }

        [Test]
        public void EditAccount_CurrentAccountsDoNotContainGivenAccount_NoInteracion()
        {
            _httpAccountEditCommand.Execute(new HttpAccount());

            _interactionRequest.AssertWasNotRaised<HttpAccountInteraction>();
        }

        [Test]
        public void EditAccount_ExecuteWithCurrentAccount_RaisesInteraction()
        {
            _httpAccountEditCommand.Execute(_currentHttpAccount);

            _interactionRequest.AssertWasRaised<HttpAccountInteraction>();
        }

        [Test]
        public void EditAccount_ExecuteWithCurrentAccount_RaisedInteractionHasCorrectTitle()
        {
            _httpAccountEditCommand.Execute(_currentHttpAccount);

            var interaction = _interactionRequest.AssertWasRaised<HttpAccountInteraction>();
            Assert.AreEqual(_translation.EditHttpAccount, interaction.Title);
        }

        [Test]
        public void EditAccount_UserApplies_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _httpAccountEditCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;
            _interactionRequest.RegisterInteractionHandler<HttpAccountInteraction>(i =>
            {
                i.Success = true; //User applies
            });

            _httpAccountEditCommand.Execute(_currentHttpAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void EditAccount_UserCancels_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _httpAccountEditCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;
            _interactionRequest.RegisterInteractionHandler<HttpAccountInteraction>(i =>
            {
                i.Success = false; //User cancels
            });

            _httpAccountEditCommand.Execute(_currentHttpAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void EditAccount_UserCancelsInteraction_CurrentAccountRemainsUnchanged()
        {
            var currentAccountCopy = _currentHttpAccount.Copy();
            _interactionRequest.RegisterInteractionHandler<HttpAccountInteraction>(i =>
            {
                i.HttpAccount.UserName = "Unwanted Changes"; // User makes changes
                i.Success = false; // User cancels
            });

            _httpAccountEditCommand.Execute(_currentHttpAccount);

            Assert.AreEqual(_currentHttpAccount, currentAccountCopy);
        }

        [Test]
        public void EditAccount_UserAppliesInteraction_CurrentAccountContainsChanges()
        {
            _interactionRequest.RegisterInteractionHandler<HttpAccountInteraction>(i =>
            {
                i.HttpAccount.UserName = "Changed Username"; // User makes changes
                i.Success = true;
            });

            _httpAccountEditCommand.Execute(_currentHttpAccount);

            var interaction = _interactionRequest.AssertWasRaised<HttpAccountInteraction>();
            Assert.AreEqual(_currentHttpAccount, interaction.HttpAccount);
            Assert.AreEqual("Changed Username", _currentHttpAccount.UserName);
        }
    }
}
