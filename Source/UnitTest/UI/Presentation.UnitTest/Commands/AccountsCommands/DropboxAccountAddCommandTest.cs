using NSubstitute;
using NUnit.Framework;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation;
using pdfforge.PDFCreator.UI.Presentation.Commands;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UnitTest.UnitTestHelper;
using pdfforge.PDFCreator.Utilities.Threading;
using System.Collections.ObjectModel;
using System.Linq;
using Translatable;

namespace Presentation.UnitTest.Commands.AccountsCommands
{
    [TestFixture]
    public class DropboxAccountAddCommandTest
    {
        private IWaitableCommand _dropboxAccountAddCommand;
        private IInteractionInvoker _interactionInvoker;
        private UnitTestInteractionRequest _interactionRequest;
        private ObservableCollection<DropboxAccount> _dropboxAccounts;
        private ICurrentSettings<Accounts> _accountsProvider;
        private DropboxTranslation _translation;

        [SetUp]
        public void SetUp()
        {
            _interactionInvoker = Substitute.For<IInteractionInvoker>();
            _interactionRequest = new UnitTestInteractionRequest();
            _dropboxAccounts = new ObservableCollection<DropboxAccount>();
            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.DropboxAccounts = _dropboxAccounts;
            var currentSettingsProvider = Substitute.For<ICurrentSettingsProvider>();
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _dropboxAccountAddCommand = new DropboxAccountAddCommand(_interactionInvoker, _interactionRequest, _accountsProvider, translationUpdater);

            _translation = new DropboxTranslation();
        }

        [Test]
        public void CanExcute_IsAlwaysTrue()
        {
            Assert.IsTrue(_dropboxAccountAddCommand.CanExecute(null));
        }

        [Test]
        public void AddAccount_InteractionInvokerInvokesInteraction()
        {
            _dropboxAccountAddCommand.Execute(null);

            _interactionInvoker.Received().Invoke(Arg.Any<DropboxAccountInteraction>());
        }

        [Test]
        public void AddAccount_UserCancelsInteraction_NoAccountGetsAdded()
        {
            _interactionInvoker.Invoke(Arg.Do<DropboxAccountInteraction>(i =>
            {
                i.Result = DropboxAccountInteractionResult.UserCanceled;
            }));

            _dropboxAccountAddCommand.Execute(null);

            Assert.IsEmpty(_dropboxAccounts);
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_AccountGetsAdded()
        {
            _interactionInvoker.Invoke(Arg.Do<DropboxAccountInteraction>(i =>
            {
                i.Result = DropboxAccountInteractionResult.Success; //User applies
                i.DropboxAccount = new DropboxAccount();
                i.DropboxAccount.AccountInfo = "Some value set in Interaction";
            }));

            _dropboxAccountAddCommand.Execute(null);

            Assert.AreEqual(1, _dropboxAccounts.Count);
            Assert.AreEqual("Some value set in Interaction", _dropboxAccounts.First().AccountInfo);
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_NewAccountHasExistingID_NewAccountReplacesOldAccountInAccountsList()
        {
            const string existingID = "ExistingID";
            var oldAccount = new DropboxAccount { AccountId = existingID };
            _dropboxAccounts.Add(oldAccount);

            var newAccount = new DropboxAccount { AccountId = existingID };

            _interactionInvoker.Invoke(Arg.Do<DropboxAccountInteraction>(i =>
            {
                i.Result = DropboxAccountInteractionResult.Success; //User applies
                i.DropboxAccount = newAccount;
            }));

            _dropboxAccountAddCommand.Execute(null);

            Assert.AreEqual(1, _dropboxAccounts.Count);
            Assert.AreSame(newAccount, _dropboxAccounts.First());
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_AccesTokenParsingErrorOccurs_InteractionRequestRaisesCorrespondingInteraction()
        {
            var newAccount = new DropboxAccount();
            _interactionInvoker.Invoke(Arg.Do<DropboxAccountInteraction>(i =>
            {
                i.Result = DropboxAccountInteractionResult.AccesTokenParsingError;
                i.DropboxAccount = newAccount;
            }));

            _dropboxAccountAddCommand.Execute(null);

            var messageInteraction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(messageInteraction.Text, _translation.DropboxAccountSeverResponseErrorMessage);
            Assert.AreEqual(messageInteraction.Title, _translation.AddDropboxAccount);
            Assert.AreEqual(messageInteraction.Buttons, MessageOptions.OK);
            Assert.AreEqual(messageInteraction.Icon, MessageIcon.Warning);
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_AccesTokenParsingErrorOccurs_AccountsRemainUntouched()
        {
            var newAccount = new DropboxAccount();
            _interactionInvoker.Invoke(Arg.Do<DropboxAccountInteraction>(i =>
            {
                i.Result = DropboxAccountInteractionResult.AccesTokenParsingError;
                i.DropboxAccount = newAccount;
            }));
            var wasCalled = false;
            _dropboxAccounts.CollectionChanged += (sender, args) => { wasCalled = true; };

            _dropboxAccountAddCommand.Execute(null);

            Assert.IsFalse(wasCalled);
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_UnknownErrorOccurs_InteractionRequestRaisesCorrespondingInteraction()
        {
            var newAccount = new DropboxAccount();
            _interactionInvoker.Invoke(Arg.Do<DropboxAccountInteraction>(i =>
            {
                i.Result = DropboxAccountInteractionResult.Error;
                i.DropboxAccount = newAccount;
            }));

            _dropboxAccountAddCommand.Execute(null);

            var messageInteraction = _interactionRequest.AssertWasRaised<MessageInteraction>();
            Assert.AreEqual(messageInteraction.Text, _translation.DropboxAccountCreationErrorMessage);
            Assert.AreEqual(messageInteraction.Title, _translation.AddDropboxAccount);
            Assert.AreEqual(messageInteraction.Buttons, MessageOptions.OK);
            Assert.AreEqual(messageInteraction.Icon, MessageIcon.Warning);
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_UnknownErrorOccurs__AccountsRemainUntouched()
        {
            var newAccount = new DropboxAccount();
            _interactionInvoker.Invoke(Arg.Do<DropboxAccountInteraction>(i =>
            {
                i.Result = DropboxAccountInteractionResult.Error;
                i.DropboxAccount = newAccount;
            }));
            var wasCalled = false;
            _dropboxAccounts.CollectionChanged += (sender, args) => { wasCalled = true; };

            _dropboxAccountAddCommand.Execute(null);

            Assert.IsFalse(wasCalled);
        }

        [Test]
        public void AddAccount_UserCancelsInteraction_DoneIsCalledWithCancel()
        {
            var wasCalled = false;
            _dropboxAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;

            _interactionInvoker.Invoke(Arg.Do<DropboxAccountInteraction>(i =>
            {
                i.Result = DropboxAccountInteractionResult.UserCanceled;
            }));

            _dropboxAccountAddCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void AddAccount_UserApplies_DoneIsCalledWithSuccess()
        {
            var wasCalled = false;
            _dropboxAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;

            _interactionInvoker.Invoke(Arg.Do<DropboxAccountInteraction>(i =>
            {
                i.Result = DropboxAccountInteractionResult.Success;
            }));

            _dropboxAccountAddCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void AddAccount_AccesTokenParsingErrorOccurs_DoneIsCalledWithError()
        {
            var wasCalled = false;
            _dropboxAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Error;

            _interactionInvoker.Invoke(Arg.Do<DropboxAccountInteraction>(i =>
            {
                i.Result = DropboxAccountInteractionResult.AccesTokenParsingError;
            }));

            _dropboxAccountAddCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void AddAccount_ErrorOccurs_DoneIsCalledWithError()
        {
            var wasCalled = false;
            _dropboxAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Error;

            _interactionInvoker.Invoke(Arg.Do<DropboxAccountInteraction>(i =>
            {
                i.Result = DropboxAccountInteractionResult.Error;
            }));

            _dropboxAccountAddCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }
    }
}
