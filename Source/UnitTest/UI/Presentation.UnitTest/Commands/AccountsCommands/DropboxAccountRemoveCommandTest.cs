using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Dropbox;
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
using System;
using System.Collections.ObjectModel;
using System.Text;
using Translatable;

namespace Presentation.UnitTest.Commands.AccountsCommands
{
    [TestFixture]
    public class DropboxAccountRemoveCommandTest
    {
        private DropboxAccountRemoveCommand _dropboxAccountRemoveCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private DropboxTranslation _translation;
        private IDropboxService _dropboxService;
        private ObservableCollection<DropboxAccount> _dropboxAccounts;
        private ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private DropboxAccount _usedAccount;
        private DropboxAccount _unusedAccount;
        private ObservableCollection<ConversionProfile> _profiles;
        private ConversionProfile _profileWithDropboxActionEnabled;
        private ConversionProfile _profileWithDropboxActionDisabled;
        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _translation = new DropboxTranslation();

            _dropboxAccounts = new ObservableCollection<DropboxAccount>();

            _usedAccount = new DropboxAccount();
            _usedAccount.AccountId = nameof(_usedAccount);
            _usedAccount.AccountInfo = "AI1";
            _usedAccount.AccessToken = "AT1";
            _dropboxAccounts.Add(_usedAccount);

            _unusedAccount = new DropboxAccount();
            _unusedAccount.AccountId = nameof(_unusedAccount);
            _unusedAccount.AccountInfo = "AI2";
            _unusedAccount.AccessToken = "AT2";
            _dropboxAccounts.Add(_unusedAccount);

            _profilesProvider = Substitute.For<ICurrentSettings<ObservableCollection<ConversionProfile>>>();
            _profiles = new ObservableCollection<ConversionProfile>();

            _profileWithDropboxActionEnabled = new ConversionProfile();
            _profileWithDropboxActionEnabled.Name = nameof(_profileWithDropboxActionEnabled);
            _profileWithDropboxActionEnabled.DropboxSettings.Enabled = true;
            _profileWithDropboxActionEnabled.DropboxSettings.AccountId = _usedAccount.AccountId;
            _profiles.Add(_profileWithDropboxActionEnabled);

            _profileWithDropboxActionDisabled = new ConversionProfile();
            _profileWithDropboxActionDisabled.Name = nameof(_profileWithDropboxActionDisabled);
            _profileWithDropboxActionDisabled.DropboxSettings.Enabled = false;
            _profileWithDropboxActionDisabled.DropboxSettings.AccountId = _usedAccount.AccountId;
            _profiles.Add(_profileWithDropboxActionDisabled);

            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.DropboxAccounts = _dropboxAccounts;
            settings.ConversionProfiles = _profiles;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);
            _profilesProvider.Settings.Returns(_profiles);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _dropboxService = Substitute.For<IDropboxService>();

            _dropboxAccountRemoveCommand = new DropboxAccountRemoveCommand(_interactionRequest, _accountsProvider, _profilesProvider, _dropboxService, translationUpdater);
        }

        [Test]
        public void CanExecute_AccountsListIsNull_DoesNotThrowException()
        {
            _dropboxAccounts = null;
            Assert.DoesNotThrow(() => _dropboxAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExcute_AccountListIsEmpty_ReturnsFalse()
        {
            _dropboxAccounts.Clear();

            Assert.IsFalse(_dropboxAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExcute_AccountListNotEmpty_ReturnsTrue()
        {
            Assert.IsTrue(_dropboxAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void Execute_GivenParameterIsNoDropboxAccount_NoInteraction()
        {
            _dropboxAccountRemoveCommand.Execute(null);

            _interactionRequest.AssertWasNotRaised<MessageInteraction>();
        }

        [Test]
        public void Execute_WithUnusedDropboxAccount_RaisedMessageInteractionHasCorrectTitleIconButtonsAndMessage()
        {
            _dropboxAccountRemoveCommand.Execute(_unusedAccount);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.RemoveDropboxAccount, interaction.Title, "Title");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");
            Assert.AreEqual(MessageIcon.Question, interaction.Icon, "Icon");

            var sb = new StringBuilder();
            sb.AppendLine(_unusedAccount.AccountInfo);
            sb.AppendLine(_translation.SureYouWantToDeleteAccount);
            var message = sb.ToString();
            Assert.AreEqual(message, interaction.Text);
        }

        [Test]
        public void Execute_WithUnusedDropboxAccount_UserCancelsInteraction_AccountRemainsInAccountsList()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _dropboxAccountRemoveCommand.Execute(_unusedAccount);

            Assert.AreEqual(2, _dropboxAccounts.Count);
            Assert.Contains(_unusedAccount, _dropboxAccounts);
        }

        [Test]
        public void Execute_WithUnusedDropboxAccount_UserAppliesInteraction_AccountGetsDeletedInAccountsList()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _dropboxAccountRemoveCommand.Execute(_unusedAccount);

            Assert.AreEqual(1, _dropboxAccounts.Count);
            Assert.IsFalse(_dropboxAccounts.Contains(_unusedAccount));
        }

        [Test]
        public void Execute_WithUsedDropboxAccount_RaisedMessageInteractionHasCorrectTitleIconButtonsAndMessage()
        {
            var numberOfProfiles = _profiles.Count;

            _dropboxAccountRemoveCommand.Execute(_usedAccount);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.RemoveDropboxAccount, interaction.Title, "Title");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Icon");

            var sb = new StringBuilder();
            sb.AppendLine(_usedAccount.AccountInfo);
            sb.AppendLine(_translation.SureYouWantToDeleteAccount);
            sb.AppendLine();
            sb.AppendLine(_translation.GetAccountIsUsedInFollowingMessage(numberOfProfiles));
            sb.AppendLine();
            sb.AppendLine(_profileWithDropboxActionEnabled.Name);
            sb.AppendLine(_profileWithDropboxActionDisabled.Name);
            sb.AppendLine();
            sb.AppendLine(_translation.GetDropboxGetsDisabledMessage(numberOfProfiles));
            var message = sb.ToString();
            Assert.AreEqual(message, interaction.Text);
        }

        [Test]
        public void Execute_WithUsedDropboxAccount_UserCancelsInteraction_AccountRemainsInAccountsListAndProfiles()
        {
            var numberOfProfiles = _profiles.Count;

            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _dropboxAccountRemoveCommand.Execute(_usedAccount);

            Assert.AreEqual(numberOfProfiles, _dropboxAccounts.Count);
            Assert.Contains(_unusedAccount, _dropboxAccounts);
            Assert.IsTrue(_profileWithDropboxActionEnabled.DropboxSettings.Enabled);
            Assert.AreEqual(_usedAccount.AccountId, _profileWithDropboxActionEnabled.DropboxSettings.AccountId);
            Assert.IsFalse(_profileWithDropboxActionDisabled.DropboxSettings.Enabled);
            Assert.AreEqual(_usedAccount.AccountId, _profileWithDropboxActionDisabled.DropboxSettings.AccountId);
        }

        [Test]
        public void Execute_WithUsedDropboxAccount_UserAppliesInteraction_AccountsDeletedInAccountsListAndFromProfiles()
        {
            var numberOfProfiles = _profiles.Count;

            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _dropboxAccountRemoveCommand.Execute(_usedAccount);

            Assert.AreEqual(numberOfProfiles - 1, _dropboxAccounts.Count);
            Assert.IsFalse(_dropboxAccounts.Contains(_usedAccount));
            Assert.IsFalse(_profileWithDropboxActionEnabled.DropboxSettings.Enabled);
            Assert.AreEqual("", _profileWithDropboxActionEnabled.DropboxSettings.AccountId);
            Assert.IsFalse(_profileWithDropboxActionDisabled.DropboxSettings.Enabled);
            Assert.AreEqual("", _profileWithDropboxActionDisabled.DropboxSettings.AccountId);
        }

        [Test]
        public void Execute_UserAppliesInteraction_DropboxServiveRevokesToken()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _dropboxAccountRemoveCommand.Execute(_usedAccount);

            _dropboxService.Received(1).RevokeToken(_usedAccount.AccessToken);
        }

        [Test]
        public void Execute_UserAppliesInteraction_DropboxServiveRevokeTokenThrowsException_ExceptionIsCatched()
        {
            _dropboxService.RevokeToken(Arg.Do<string>(s => { throw new Exception(); }));

            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            Assert.DoesNotThrow(() => _dropboxAccountRemoveCommand.Execute(_usedAccount));
        }

        [Test]
        public void Execute_UserCancelsInteraction_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _dropboxAccountRemoveCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _dropboxAccountRemoveCommand.Execute(_unusedAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Execute_UserAppliesInteraction_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _dropboxAccountRemoveCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _dropboxAccountRemoveCommand.Execute(_unusedAccount);

            Assert.IsTrue(wasCalled);
        }
    }
}
