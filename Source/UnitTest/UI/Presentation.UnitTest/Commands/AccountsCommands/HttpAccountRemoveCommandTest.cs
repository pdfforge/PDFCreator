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
    public class HttpAccountRemoveCommandTest
    {
        private HttpAccountRemoveCommand _httpAccountRemoveCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private HttpTranslation _translation;
        private ObservableCollection<HttpAccount> _httpAccounts;
        private HttpAccount _usedAccount;
        private HttpAccount _unusedAccount;
        private ObservableCollection<ConversionProfile> _profiles;
        private ConversionProfile _profileWithHttpAccountEnabled;
        private ConversionProfile _profileWithHttpAccountDisabled;
        private ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _translation = new HttpTranslation();
            _profilesProvider = Substitute.For<ICurrentSettings<ObservableCollection<ConversionProfile>>>();
            _httpAccounts = new ObservableCollection<HttpAccount>();

            _usedAccount = new HttpAccount();
            _usedAccount.AccountId = nameof(_usedAccount);
            _usedAccount.Url = "www.pdfforge1.org";
            _usedAccount.UserName = "UN1";
            _httpAccounts.Add(_usedAccount);

            _unusedAccount = new HttpAccount();
            _unusedAccount.AccountId = nameof(_unusedAccount);
            _unusedAccount.Url = "www.pdfforge2.org";
            _unusedAccount.UserName = "UN2";
            _httpAccounts.Add(_unusedAccount);

            _profiles = new ObservableCollection<ConversionProfile>();

            _profileWithHttpAccountEnabled = new ConversionProfile();
            _profileWithHttpAccountEnabled.Name = nameof(_profileWithHttpAccountEnabled);
            _profileWithHttpAccountEnabled.HttpSettings.Enabled = true;
            _profileWithHttpAccountEnabled.HttpSettings.AccountId = _usedAccount.AccountId;
            _profiles.Add(_profileWithHttpAccountEnabled);

            _profileWithHttpAccountDisabled = new ConversionProfile();
            _profileWithHttpAccountDisabled.Name = nameof(_profileWithHttpAccountDisabled);
            _profileWithHttpAccountDisabled.HttpSettings.Enabled = false;
            _profileWithHttpAccountDisabled.HttpSettings.AccountId = _usedAccount.AccountId;
            _profiles.Add(_profileWithHttpAccountDisabled);

            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.HttpAccounts = _httpAccounts;
            settings.ConversionProfiles = _profiles;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);
            _profilesProvider.Settings.Returns(_profiles);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _httpAccountRemoveCommand = new HttpAccountRemoveCommand(_interactionRequest, _accountsProvider, _profilesProvider, translationUpdater);
        }

        [Test]
        public void CanExecute_AccountsListIsNull_DoesNotThrowException()
        {
            _httpAccounts = null;
            Assert.DoesNotThrow(() => _httpAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListIsEmpty_ReturnsFalse()
        {
            _httpAccounts.Clear();

            Assert.IsFalse(_httpAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListHasAccount_ReturnsTrue()
        {
            Assert.IsTrue(_httpAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void Execute_GivenParameterIsNotHttpAccount_NoInteraction()
        {
            _httpAccountRemoveCommand.Execute(null);

            _interactionRequest.AssertWasNotRaised<HttpAccountInteraction>();
        }

        [Test]
        public void Execute_WithUnusedHttpAccount_RaisedMessageInteractionHasCorrectTitleIconButtonsAndMessage()
        {
            _httpAccountRemoveCommand.Execute(_unusedAccount);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.RemoveHttpAccount, interaction.Title, "Title");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");
            Assert.AreEqual(MessageIcon.Question, interaction.Icon, "Icon");

            var sb = new StringBuilder();
            sb.AppendLine(_unusedAccount.AccountInfo);
            sb.AppendLine(_translation.SureYouWantToDeleteAccount);
            var message = sb.ToString();
            Assert.AreEqual(message, interaction.Text);
        }

        [Test]
        public void Execute_WithUnusedHttpAccount_UserCancelsInteraction_AccountRemainsInAccounts()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _httpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.AreEqual(2, _httpAccounts.Count);
            Assert.Contains(_unusedAccount, _httpAccounts);
        }

        [Test]
        public void Execute_WithUnusedHttpAccount_UserAppliesInteraction_AccountIsRemovedFromAccounts()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _httpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.AreEqual(1, _httpAccounts.Count);
            Assert.IsFalse(_httpAccounts.Contains(_unusedAccount));
        }

        [Test]
        public void Execute_WithUsedHttpAccount_RaisedMessageInteractionHasCorrectTitleIconButtonsAndMessage()
        {
            var numberOfProfiles = _profiles.Count;
            _httpAccountRemoveCommand.Execute(_usedAccount);
            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.RemoveHttpAccount, interaction.Title, "Title");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Icon");

            var sb = new StringBuilder();
            sb.AppendLine(_usedAccount.AccountInfo);
            sb.AppendLine(_translation.SureYouWantToDeleteAccount);
            sb.AppendLine();
            sb.AppendLine(_translation.GetAccountIsUsedInFollowingMessage(numberOfProfiles));
            sb.AppendLine();
            sb.AppendLine(_profileWithHttpAccountEnabled.Name);
            sb.AppendLine(_profileWithHttpAccountDisabled.Name);
            sb.AppendLine();
            sb.AppendLine(_translation.GetHttppGetsDisabledMessage(numberOfProfiles));
            var message = sb.ToString();
            Assert.AreEqual(message, interaction.Text);
        }

        [Test]
        public void Execute_WithUsedHttpAccount_UserCancelsInteraction_AccountRemainsInAccountsAndProfiles()
        {
            var numberOfProfiles = _profiles.Count;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _httpAccountRemoveCommand.Execute(_usedAccount);

            Assert.AreEqual(numberOfProfiles, _httpAccounts.Count);
            Assert.Contains(_usedAccount, _httpAccounts);
            Assert.IsTrue(_profileWithHttpAccountEnabled.HttpSettings.Enabled);
            Assert.AreEqual(_usedAccount.AccountId, _profileWithHttpAccountEnabled.HttpSettings.AccountId);
            Assert.IsFalse(_profileWithHttpAccountDisabled.HttpSettings.Enabled);
            Assert.AreEqual(_usedAccount.AccountId, _profileWithHttpAccountDisabled.HttpSettings.AccountId);
        }

        [Test]
        public void Execute_WithUsedHttpAccount_UserAppliesInteraction_AccountsDeletedInAccountsAndInProfiles()
        {
            var numberOfProfiles = _profiles.Count;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _httpAccountRemoveCommand.Execute(_usedAccount);

            Assert.AreEqual(numberOfProfiles - 1, _httpAccounts.Count);
            Assert.IsFalse(_httpAccounts.Contains(_usedAccount));
            Assert.IsFalse(_profileWithHttpAccountEnabled.HttpSettings.Enabled);
            Assert.AreEqual("", _profileWithHttpAccountEnabled.HttpSettings.AccountId);
            Assert.IsFalse(_profileWithHttpAccountDisabled.HttpSettings.Enabled);
            Assert.AreEqual("", _profileWithHttpAccountDisabled.HttpSettings.AccountId);
        }

        [Test]
        public void Execute_UserCancelsInteraction_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _httpAccountRemoveCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _httpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Execute_UserAppliesInteraction_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _httpAccountRemoveCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _httpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.IsTrue(wasCalled);
        }
    }
}
