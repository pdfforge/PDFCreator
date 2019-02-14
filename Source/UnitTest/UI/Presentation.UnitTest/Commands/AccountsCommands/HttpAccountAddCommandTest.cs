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
using System.Windows.Data;
using Translatable;

namespace Presentation.UnitTest.Commands.AccountsCommands
{
    [TestFixture]
    public class HttpAccountAddCommandTest
    {
        private IWaitableCommand _httpAccountAddCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ObservableCollection<HttpAccount> _httpAccounts;
        private readonly AccountsTranslation _translation = new AccountsTranslation();
        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();

            _httpAccounts = new ObservableCollection<HttpAccount>();
            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.HttpAccounts = _httpAccounts;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _httpAccountAddCommand = new HttpAccountAddCommand(_interactionRequest, _accountsProvider, translationUpdater);
        }

        [Test]
        public void CanExecute_AccountsListIsNull_DoesNotThrowException()
        {
            _httpAccounts = null;
            Assert.DoesNotThrow(() => _httpAccountAddCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_IsAlwaysTrue()
        {
            Assert.IsTrue(_httpAccountAddCommand.CanExecute(null));
        }

        [Test]
        public void AddAccount_InteractionRequestRaisesHttpAccountInteraction()
        {
            _httpAccountAddCommand.Execute(null);

            _interactionRequest.AssertWasRaised<HttpAccountInteraction>();
        }

        [Test]
        public void AddAccount_RaisedInteractionHasCorrectTitle()
        {
            _httpAccountAddCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<HttpAccountInteraction>();

            Assert.AreEqual(_translation.AddHttpAccount, interaction.Title, "Wrong Title for HttpAccountInteraction");
        }

        [Test]
        public void AddAccount_RaisedInteractionContainsAccountWithID()
        {
            _httpAccountAddCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<HttpAccountInteraction>();
            Assert.IsFalse(string.IsNullOrWhiteSpace(interaction.HttpAccount.AccountId), "Raised interaction contains account without ID");
        }

        [Test]
        public void AddAccount_UserCancelsInteraction_NoAccountGetsAdded()
        {
            _interactionRequest.RegisterInteractionHandler<HttpAccountInteraction>(i =>
            {
                i.Success = false; // User cancels
            });

            _httpAccountAddCommand.Execute(null);

            Assert.IsEmpty(_httpAccounts);
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_AccountGetsAdded()
        {
            var newAccount = new HttpAccount();
            newAccount.Url = "New Url";
            newAccount.Password = "New Password";
            newAccount.UserName = "New Username";

            _interactionRequest.RegisterInteractionHandler<HttpAccountInteraction>(i =>
            {
                i.Success = true;
                i.HttpAccount = newAccount;
            });

            _httpAccountAddCommand.Execute(null);

            Assert.AreEqual(1, _httpAccounts.Count);
            Assert.AreSame(newAccount, _httpAccounts.FirstOrDefault());
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_AccountIsCurrentItemInView()
        {
            var collectionView = CollectionViewSource.GetDefaultView(_httpAccounts);

            HttpAccount newAccount = new HttpAccount();

            _interactionRequest.RegisterInteractionHandler<HttpAccountInteraction>(i =>
            {
                i.Success = true;
                newAccount = i.HttpAccount;
            });

            _httpAccountAddCommand.Execute(null);

            Assert.AreSame(collectionView.CurrentItem, newAccount);
        }

        [Test]
        public void AddAccount_UserCancelsInteraction_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _httpAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;

            _interactionRequest.RegisterInteractionHandler<HttpAccountInteraction>(i =>
            {
                i.Success = false; // User cancels
            });

            _httpAccountAddCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _httpAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;

            _interactionRequest.RegisterInteractionHandler<HttpAccountInteraction>(i =>
            {
                i.Success = true; // User applies
            });

            _httpAccountAddCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }
    }
}
