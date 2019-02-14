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
    public class FtpAccountEditCommandTest
    {
        private IWaitableCommand _ftpAccountEditCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ObservableCollection<FtpAccount> _ftpAccounts;
        private FtpAccount _currentFtpAccount;
        private FtpActionTranslation _translation;
        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _currentFtpAccount = new FtpAccount();
            _currentFtpAccount.UserName = "CurrentUserName";
            _ftpAccounts = new ObservableCollection<FtpAccount>();
            _ftpAccounts.Add(_currentFtpAccount);
            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.FtpAccounts = _ftpAccounts;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            _translation = new FtpActionTranslation();
            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _ftpAccountEditCommand = new FtpAccountEditCommand(_interactionRequest, _accountsProvider, translationUpdater);
        }

        [Test]
        public void CanExecute_AccountsListIsNull_DoesNotThrowException()
        {
            _ftpAccounts = null;
            Assert.DoesNotThrow(() => _ftpAccountEditCommand.CanExecute(null));
        }

        [Test]
        public void CanExcute_WithAccountsEmpty_ReturnsFalse()
        {
            _ftpAccounts.Clear();
            Assert.IsFalse(_ftpAccountEditCommand.CanExecute(null));
        }

        [Test]
        public void CanExcute_WithAccountParameter_ReturnsTrue()
        {
            Assert.IsTrue(_ftpAccountEditCommand.CanExecute(new FtpAccount()));
        }

        [Test]
        public void EditAccount_GivenParameterIsNoFtpAccount_NoInteraction()
        {
            _ftpAccountEditCommand.Execute(null);

            _interactionRequest.AssertWasNotRaised<FtpAccountInteraction>();
        }

        [Test]
        public void EditAccount_CurrentAccountsDoNotContainGivenAccount_NoInteraction()
        {
            _ftpAccountEditCommand.Execute(new FtpAccount());

            _interactionRequest.AssertWasNotRaised<FtpAccountInteraction>();
        }

        [Test]
        public void EditAccount_ExecuteWithCurrentAccount_RaisesInteraction()
        {
            _ftpAccountEditCommand.Execute(_currentFtpAccount);

            _interactionRequest.AssertWasRaised<FtpAccountInteraction>();
        }

        [Test]
        public void EditAccount_ExecuteWithCurrentAccount_RaisedInteractionHasCorrectTitle()
        {
            _ftpAccountEditCommand.Execute(_currentFtpAccount);

            var interaction = _interactionRequest.AssertWasRaised<FtpAccountInteraction>();
            Assert.AreEqual(_translation.EditFtpAccount, interaction.Title);
        }

        [Test]
        public void EditAccount_UserApplies_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _ftpAccountEditCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;
            _interactionRequest.RegisterInteractionHandler<FtpAccountInteraction>(i =>
            {
                i.Success = true; //User applies
            });

            _ftpAccountEditCommand.Execute(_currentFtpAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void EditAccount_UserCancels_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _ftpAccountEditCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;
            _interactionRequest.RegisterInteractionHandler<FtpAccountInteraction>(i =>
            {
                i.Success = false; //User cancels
            });

            _ftpAccountEditCommand.Execute(_currentFtpAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void EditAccount_UserCancelsInteraction_CurrentAccountRemainsUnchanged()
        {
            var currentAccountCopy = _currentFtpAccount.Copy();
            _interactionRequest.RegisterInteractionHandler<FtpAccountInteraction>(i =>
            {
                i.FtpAccount.UserName = "Unwanted Changes"; //User does changes
                i.Success = false; //User cancels
            });

            _ftpAccountEditCommand.Execute(_currentFtpAccount);

            Assert.AreEqual(_currentFtpAccount, currentAccountCopy);
        }

        [Test]
        public void EditAccount_UserAppliesInteraction_CurrentAccountContainsChanges()
        {
            _interactionRequest.RegisterInteractionHandler<FtpAccountInteraction>(i =>
            {
                i.FtpAccount.UserName = "Changed UserName"; //User does changes
                i.Success = true; //User applies
            });

            _ftpAccountEditCommand.Execute(_currentFtpAccount);

            var interaction = _interactionRequest.AssertWasRaised<FtpAccountInteraction>();
            Assert.AreEqual(_currentFtpAccount, interaction.FtpAccount);
            Assert.AreEqual("Changed UserName", _currentFtpAccount.UserName);
        }

        [Test]
        public void EditAccount_UserAppliesInteraction_RaisePropertyChangedOfCurrentAccount()
        {
            var wasCalled = false;
            _ftpAccounts[0].PropertyChanged += (sender, args) => wasCalled = true;

            _interactionRequest.RegisterInteractionHandler<FtpAccountInteraction>(i =>
            {
                i.FtpAccount.UserName = "Changed UserName"; //User does changes
                i.Success = true; //User applies
            });

            _ftpAccountEditCommand.Execute(_currentFtpAccount);

            Assert.IsTrue(wasCalled);
        }
    }
}
