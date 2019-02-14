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
    public class TimeServerAccountEditCommandTest
    {
        private TimeServerAccountEditCommand _timeServerAccountEditCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ObservableCollection<TimeServerAccount> _timeServerAccounts;
        private TimeServerAccount _currentTimeServerAccount;
        private TimeServerTranslation _translation;
        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _translation = new TimeServerTranslation();
            _currentTimeServerAccount = new TimeServerAccount { UserName = "CurrentUsername" };

            _timeServerAccounts = new ObservableCollection<TimeServerAccount>();
            _timeServerAccounts.Add(_currentTimeServerAccount);

            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.TimeServerAccounts = _timeServerAccounts;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());
            _timeServerAccountEditCommand = new TimeServerAccountEditCommand(_interactionRequest, _accountsProvider, translationUpdater);
        }

        [Test]
        public void CanExecute_AccountsListIsNull_DoesNotThrowException()
        {
            _timeServerAccounts = null;

            Assert.DoesNotThrow(() => _timeServerAccountEditCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListIsEmpty_ReturnsFalse()
        {
            _timeServerAccounts.Clear();

            Assert.IsFalse(_timeServerAccountEditCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListHasAccount_ReturnsTrue()
        {
            Assert.IsTrue(_timeServerAccountEditCommand.CanExecute(null));
        }

        [Test]
        public void EditAccount_GivenParameterIsNotTimeServerAccount_NoInteraction()
        {
            _timeServerAccountEditCommand.Execute(null);

            _interactionRequest.AssertWasNotRaised<TimeServerAccountInteraction>();
        }

        [Test]
        public void EditAccount_CurrentAccountsDoNotContainGivenAccount_NoInteracion()
        {
            _timeServerAccountEditCommand.Execute(new TimeServerAccount());

            _interactionRequest.AssertWasNotRaised<TimeServerAccountInteraction>();
        }

        [Test]
        public void EditAccount_ExecuteWithCurrentAccount_RaisesInteraction()
        {
            _timeServerAccountEditCommand.Execute(_currentTimeServerAccount);

            _interactionRequest.AssertWasRaised<TimeServerAccountInteraction>();
        }

        [Test]
        public void EditAccount_ExecuteWithCurrentAccount_RaisedInteractionHasCorrectTitle()
        {
            _timeServerAccountEditCommand.Execute(_currentTimeServerAccount);

            var interaction = _interactionRequest.AssertWasRaised<TimeServerAccountInteraction>();
            Assert.AreEqual(_translation.EditTimeServerAccount, interaction.Title);
        }

        [Test]
        public void EditAccount_UserApplies_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _timeServerAccountEditCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;
            _interactionRequest.RegisterInteractionHandler<TimeServerAccountInteraction>(i =>
            {
                i.Success = true; //User applies
            });

            _timeServerAccountEditCommand.Execute(_currentTimeServerAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void EditAccount_UserCancels_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _timeServerAccountEditCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;
            _interactionRequest.RegisterInteractionHandler<TimeServerAccountInteraction>(i =>
            {
                i.Success = false; //User cancels
            });

            _timeServerAccountEditCommand.Execute(_currentTimeServerAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void EditAccount_UserCancelsInteraction_CurrentAccountRemainsUnchanged()
        {
            var currentAccountCopy = _currentTimeServerAccount.Copy();
            _interactionRequest.RegisterInteractionHandler<TimeServerAccountInteraction>(i =>
            {
                i.TimeServerAccount.UserName = "Unwanted Changes"; // User makes changes
                i.Success = false; // User cancels
            });

            _timeServerAccountEditCommand.Execute(_currentTimeServerAccount);

            Assert.AreEqual(_currentTimeServerAccount, currentAccountCopy);
        }

        [Test]
        public void EditAccount_UserAppliesInteraction_CurrentAccountContainsChanges()
        {
            _interactionRequest.RegisterInteractionHandler<TimeServerAccountInteraction>(i =>
            {
                i.TimeServerAccount.UserName = "Changed Username"; // User makes changes
                i.Success = true;
            });

            _timeServerAccountEditCommand.Execute(_currentTimeServerAccount);

            var interaction = _interactionRequest.AssertWasRaised<TimeServerAccountInteraction>();
            Assert.AreEqual(_currentTimeServerAccount, interaction.TimeServerAccount);
            Assert.AreEqual("Changed Username", _currentTimeServerAccount.UserName);
        }
    }
}
