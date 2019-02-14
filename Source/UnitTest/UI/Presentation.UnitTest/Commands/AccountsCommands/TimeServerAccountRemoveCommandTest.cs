using NSubstitute;
using NUnit.Framework;
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
using System.Text;
using Translatable;

namespace Presentation.UnitTest.Commands.AccountsCommands
{
    [TestFixture]
    public class TimeServerAccountDeleteCommandTest
    {
        private TimeServerAccountRemoveCommand _timeServerAccountRemoveCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private TimeServerTranslation _translation;
        private ObservableCollection<TimeServerAccount> _timeServerAccounts;
        private TimeServerAccount _usedAccount;
        private TimeServerAccount _unusedAccount;
        private ObservableCollection<ConversionProfile> _profiles;
        private ConversionProfile _profileWithTimeServerAccountEnabled;
        private ConversionProfile _profileWithTimeServerAccountDisabled;

        private ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _translation = new TimeServerTranslation();
            _profilesProvider = Substitute.For<ICurrentSettings<ObservableCollection<ConversionProfile>>>();
            _timeServerAccounts = new ObservableCollection<TimeServerAccount>();

            _usedAccount = new TimeServerAccount();
            _usedAccount.AccountId = nameof(_usedAccount);
            _usedAccount.UserName = "Username1";
            _usedAccount.Url = "Url1";
            _timeServerAccounts.Add(_usedAccount);

            _unusedAccount = new TimeServerAccount();
            _unusedAccount.AccountId = nameof(_unusedAccount);
            _unusedAccount.UserName = "Username2";
            _unusedAccount.Url = "Url2";
            _timeServerAccounts.Add(_unusedAccount);

            _profiles = new ObservableCollection<ConversionProfile>();

            _profileWithTimeServerAccountEnabled = new ConversionProfile();
            _profileWithTimeServerAccountEnabled.Name = nameof(_profileWithTimeServerAccountEnabled);
            _profileWithTimeServerAccountEnabled.PdfSettings.Signature.Enabled = true;
            _profileWithTimeServerAccountEnabled.PdfSettings.Signature.TimeServerAccountId = _usedAccount.AccountId;
            _profiles.Add(_profileWithTimeServerAccountEnabled);

            _profileWithTimeServerAccountDisabled = new ConversionProfile();
            _profileWithTimeServerAccountDisabled.Name = nameof(_profileWithTimeServerAccountDisabled);
            _profileWithTimeServerAccountDisabled.PdfSettings.Signature.Enabled = false;
            _profileWithTimeServerAccountDisabled.PdfSettings.Signature.TimeServerAccountId = _usedAccount.AccountId;
            _profiles.Add(_profileWithTimeServerAccountDisabled);

            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.TimeServerAccounts = _timeServerAccounts;
            settings.ConversionProfiles = _profiles;

            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);
            _profilesProvider.Settings.Returns(_profiles);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _timeServerAccountRemoveCommand = new TimeServerAccountRemoveCommand(_interactionRequest, _profilesProvider, _accountsProvider, translationUpdater);
        }

        [Test]
        public void CanExecute_AccountsListIsNull_DoesNotThrowException()
        {
            _timeServerAccounts = null;
            Assert.DoesNotThrow(() => _timeServerAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListIsEmpty_ReturnsFalse()
        {
            _timeServerAccounts.Clear();

            Assert.IsFalse(_timeServerAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListHasAccount_ReturnsTrue()
        {
            Assert.IsTrue(_timeServerAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void Execute_GivenParameterIsNotTimeServerAccount_NoInteraction()
        {
            _timeServerAccountRemoveCommand.Execute(null);

            _interactionRequest.AssertWasNotRaised<TimeServerAccountInteraction>();
        }

        [Test]
        public void Execute_WithUnusedTimeServerAccount_RaisedMessageInteractionHasCorrectTitleIconButtonsAndMessage()
        {
            _timeServerAccountRemoveCommand.Execute(_unusedAccount);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.RemoveTimeServerAccount, interaction.Title, "Title");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");
            Assert.AreEqual(MessageIcon.Question, interaction.Icon, "Icon");

            var sb = new StringBuilder();
            sb.AppendLine(_unusedAccount.AccountInfo);
            sb.AppendLine(_translation.SureYouWantToDeleteAccount);
            var message = sb.ToString();
            Assert.AreEqual(message, interaction.Text);
        }

        [Test]
        public void Execute_WithUnusedTimeServerAccount_UserCancelsInteraction_AccountRemainsInAccounts()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.Cancel;
            });

            _timeServerAccountRemoveCommand.Execute(_unusedAccount);

            Assert.AreEqual(2, _timeServerAccounts.Count);
            Assert.Contains(_unusedAccount, _timeServerAccounts);
        }

        [Test]
        public void Execute_WithUnusedTimeServerAccount_UserAppliesInteraction_AccountIsRemovedFromAccounts()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.Yes;
            });

            _timeServerAccountRemoveCommand.Execute(_unusedAccount);

            Assert.AreEqual(1, _timeServerAccounts.Count);
            Assert.IsFalse(_timeServerAccounts.Contains(_unusedAccount));
        }

        [Test]
        public void Execute_WithUsedTimeServerAccount_RaisedMessageInteractionHasCorrectTitleIconButtonsAndMessage()
        {
            var numberOfProfiles = _profiles.Count;
            _timeServerAccountRemoveCommand.Execute(_usedAccount);
            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.RemoveTimeServerAccount, interaction.Title, "Title");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Icon");

            var sb = new StringBuilder();
            sb.AppendLine(_usedAccount.AccountInfo);
            sb.AppendLine(_translation.SureYouWantToDeleteAccount);
            sb.AppendLine();
            sb.AppendLine(_translation.GetAccountIsUsedInFollowingMessage(numberOfProfiles));
            sb.AppendLine();
            sb.AppendLine(_profileWithTimeServerAccountEnabled.Name);
            sb.AppendLine(_profileWithTimeServerAccountDisabled.Name);
            sb.AppendLine();
            sb.AppendLine(_translation.GetTimeServerGetsDisabledMessage(numberOfProfiles));
            var message = sb.ToString();
            Assert.AreEqual(message, interaction.Text);
        }

        [Test]
        public void Execute_WithUsedTimeServerAccount_UserCancelsInteraction_AccountRemainsInAccountsAndProfiles()
        {
            var numberOfProfiles = _profiles.Count;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _timeServerAccountRemoveCommand.Execute(_usedAccount);

            Assert.AreEqual(numberOfProfiles, _timeServerAccounts.Count);
            Assert.Contains(_usedAccount, _timeServerAccounts);
            Assert.IsTrue(_profileWithTimeServerAccountEnabled.PdfSettings.Signature.Enabled);
            Assert.AreEqual(_usedAccount.AccountId, _profileWithTimeServerAccountEnabled.PdfSettings.Signature.TimeServerAccountId);
            Assert.IsFalse(_profileWithTimeServerAccountDisabled.PdfSettings.Signature.Enabled);
            Assert.AreEqual(_usedAccount.AccountId, _profileWithTimeServerAccountDisabled.PdfSettings.Signature.TimeServerAccountId);
        }

        [Test]
        public void Execute_WithUsedTimeServerAccount_UserAppliesInteraction_AccountsDeletedInAccountsAndInProfiles()
        {
            var numberOfProfiles = _profiles.Count;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _timeServerAccountRemoveCommand.Execute(_usedAccount);

            Assert.AreEqual(numberOfProfiles - 1, _timeServerAccounts.Count);
            Assert.IsFalse(_timeServerAccounts.Contains(_usedAccount));
            Assert.IsFalse(_profileWithTimeServerAccountEnabled.PdfSettings.Signature.Enabled);
            Assert.AreEqual("", _profileWithTimeServerAccountEnabled.PdfSettings.Signature.TimeServerAccountId);
            Assert.IsFalse(_profileWithTimeServerAccountDisabled.PdfSettings.Signature.Enabled);
            Assert.AreEqual("", _profileWithTimeServerAccountDisabled.PdfSettings.Signature.TimeServerAccountId);
        }

        [Test]
        public void Execute_UserCancelsInteraction_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _timeServerAccountRemoveCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _timeServerAccountRemoveCommand.Execute(_unusedAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Execute_UserAppliesInteraction_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _timeServerAccountRemoveCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _timeServerAccountRemoveCommand.Execute(_unusedAccount);

            Assert.IsTrue(wasCalled);
        }
    }
}
