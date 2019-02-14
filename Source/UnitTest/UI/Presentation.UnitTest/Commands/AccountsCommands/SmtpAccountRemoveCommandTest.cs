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
    public class SmtpAccountRemoveCommandTest
    {
        private SmtpAccountRemoveCommand _smtpAccountRemoveCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private SmtpTranslation _translation;
        private ObservableCollection<SmtpAccount> _smtpAccounts;
        private SmtpAccount _usedAccount;
        private SmtpAccount _unusedAccount;
        private ObservableCollection<ConversionProfile> _profiles;
        private ConversionProfile _profileWithSmtpAccountEnabled;
        private ConversionProfile _profileWithSmtpAccountDisabled;
        private ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();
            _translation = new SmtpTranslation();
            _profilesProvider = Substitute.For<ICurrentSettings<ObservableCollection<ConversionProfile>>>();
            _smtpAccounts = new ObservableCollection<SmtpAccount>();

            _usedAccount = new SmtpAccount();
            _usedAccount.AccountId = nameof(_usedAccount);
            _usedAccount.UserName = "UN1";
            _usedAccount.Server = "SV1";
            _smtpAccounts.Add(_usedAccount);

            _unusedAccount = new SmtpAccount();
            _unusedAccount.AccountId = nameof(_unusedAccount);
            _unusedAccount.UserName = "UN2";
            _unusedAccount.Server = "SV2";
            _smtpAccounts.Add(_unusedAccount);

            _profiles = new ObservableCollection<ConversionProfile>();

            _profileWithSmtpAccountEnabled = new ConversionProfile();
            _profileWithSmtpAccountEnabled.Name = nameof(_profileWithSmtpAccountEnabled);
            _profileWithSmtpAccountEnabled.EmailSmtpSettings.Enabled = true;
            _profileWithSmtpAccountEnabled.EmailSmtpSettings.AccountId = _usedAccount.AccountId;
            _profiles.Add(_profileWithSmtpAccountEnabled);

            _profileWithSmtpAccountDisabled = new ConversionProfile();
            _profileWithSmtpAccountDisabled.Name = nameof(_profileWithSmtpAccountDisabled);
            _profileWithSmtpAccountDisabled.EmailSmtpSettings.Enabled = false;
            _profileWithSmtpAccountDisabled.EmailSmtpSettings.AccountId = _usedAccount.AccountId;
            _profiles.Add(_profileWithSmtpAccountDisabled);

            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.SmtpAccounts = _smtpAccounts;
            settings.ConversionProfiles = _profiles;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);
            _profilesProvider.Settings.Returns(_profiles);

            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _smtpAccountRemoveCommand = new SmtpAccountRemoveCommand(_interactionRequest, _accountsProvider, _profilesProvider, translationUpdater);
        }

        [Test]
        public void CanExecute_AccountsListIsNull_DoesNotThrowException()
        {
            _smtpAccounts = null;
            Assert.DoesNotThrow(() => _smtpAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListIsEmpty_ReturnsFalse()
        {
            _smtpAccounts.Clear();

            Assert.IsFalse(_smtpAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void CanExecute_AccountListHasAccount_ReturnsTrue()
        {
            Assert.IsTrue(_smtpAccountRemoveCommand.CanExecute(null));
        }

        [Test]
        public void Execute_GivenParameterIsNotSmtpAccount_NoInteraction()
        {
            _smtpAccountRemoveCommand.Execute(null);

            _interactionRequest.AssertWasNotRaised<SmtpAccountInteraction>();
        }

        [Test]
        public void Execute_WithUnusedSmtpAccount_RaisedMessageInteractionHasCorrectTitleIconButtonsAndMessage()
        {
            _smtpAccountRemoveCommand.Execute(_unusedAccount);

            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.RemoveSmtpAccount, interaction.Title, "Title");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");
            Assert.AreEqual(MessageIcon.Question, interaction.Icon, "Icon");

            var sb = new StringBuilder();
            sb.AppendLine(_unusedAccount.AccountInfo);
            sb.AppendLine(_translation.SureYouWantToDeleteAccount);
            var message = sb.ToString();
            Assert.AreEqual(message, interaction.Text);
        }

        [Test]
        public void Execute_WithUnusedSmtpAccount_UserCancelsInteraction_AccountRemainsInAccounts()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.Cancel;
            });

            _smtpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.AreEqual(2, _smtpAccounts.Count);
            Assert.Contains(_unusedAccount, _smtpAccounts);
        }

        [Test]
        public void Execute_WithUnusedSmtpAccount_UserAppliesInteraction_AccountIsRemovedFromAccounts()
        {
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i =>
            {
                i.Response = MessageResponse.Yes;
            });

            _smtpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.AreEqual(1, _smtpAccounts.Count);
            Assert.IsFalse(_smtpAccounts.Contains(_unusedAccount));
        }

        [Test]
        public void Execute_WithUsedSmtpAccount_RaisedMessageInteractionHasCorrectTitleIconButtonsAndMessage()
        {
            var numberOfProfiles = _profiles.Count;
            _smtpAccountRemoveCommand.Execute(_usedAccount);
            var interaction = _interactionRequest.AssertWasRaised<MessageInteraction>();

            Assert.AreEqual(_translation.RemoveSmtpAccount, interaction.Title, "Title");
            Assert.AreEqual(MessageOptions.YesNo, interaction.Buttons, "Buttons");
            Assert.AreEqual(MessageIcon.Warning, interaction.Icon, "Icon");

            var sb = new StringBuilder();
            sb.AppendLine(_usedAccount.AccountInfo);
            sb.AppendLine(_translation.SureYouWantToDeleteAccount);
            sb.AppendLine();
            sb.AppendLine(_translation.GetAccountIsUsedInFollowingMessage(numberOfProfiles));
            sb.AppendLine();
            sb.AppendLine(_profileWithSmtpAccountEnabled.Name);
            sb.AppendLine(_profileWithSmtpAccountDisabled.Name);
            sb.AppendLine();
            sb.AppendLine(_translation.GetSmtpGetsDisabledMessage(numberOfProfiles));
            var message = sb.ToString();
            Assert.AreEqual(message, interaction.Text);
        }

        [Test]
        public void Execute_WithUsedSmtpAccount_UserCancelsInteraction_AccountRemainsInAccountsAndProfiles()
        {
            var numberOfProfiles = _profiles.Count;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _smtpAccountRemoveCommand.Execute(_usedAccount);

            Assert.AreEqual(numberOfProfiles, _smtpAccounts.Count);
            Assert.Contains(_usedAccount, _smtpAccounts);
            Assert.IsTrue(_profileWithSmtpAccountEnabled.EmailSmtpSettings.Enabled);
            Assert.AreEqual(_usedAccount.AccountId, _profileWithSmtpAccountEnabled.EmailSmtpSettings.AccountId);
            Assert.IsFalse(_profileWithSmtpAccountDisabled.EmailSmtpSettings.Enabled);
            Assert.AreEqual(_usedAccount.AccountId, _profileWithSmtpAccountDisabled.EmailSmtpSettings.AccountId);
        }

        [Test]
        public void Execute_WithUsedSmtpAccount_UserAppliesInteraction_AccountsDeletedInAccountsAndInProfiles()
        {
            var numberOfProfiles = _profiles.Count;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _smtpAccountRemoveCommand.Execute(_usedAccount);

            Assert.AreEqual(numberOfProfiles - 1, _smtpAccounts.Count);
            Assert.IsFalse(_smtpAccounts.Contains(_usedAccount));
            Assert.IsFalse(_profileWithSmtpAccountEnabled.EmailSmtpSettings.Enabled);
            Assert.AreEqual("", _profileWithSmtpAccountEnabled.EmailSmtpSettings.AccountId);
            Assert.IsFalse(_profileWithSmtpAccountDisabled.EmailSmtpSettings.Enabled);
            Assert.AreEqual("", _profileWithSmtpAccountDisabled.EmailSmtpSettings.AccountId);
        }

        [Test]
        public void Execute_UserCancelsInteraction_IsDoneCalledWithCancel()
        {
            var wasCalled = false;

            _smtpAccountRemoveCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Cancel);

            _smtpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void Execute_UserAppliesInteraction_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _smtpAccountRemoveCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;
            _interactionRequest.RegisterInteractionHandler<MessageInteraction>(i => i.Response = MessageResponse.Yes);

            _smtpAccountRemoveCommand.Execute(_unusedAccount);

            Assert.IsTrue(wasCalled);
        }
    }
}
