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
using System.Linq;
using Translatable;

namespace Presentation.UnitTest.Commands.AccountsCommands
{
    [TestFixture]
    public class TimeServerAccountAddCommandTest
    {
        private TimeServerAccountAddCommand _timeServerAccountAddCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ObservableCollection<TimeServerAccount> _timeServerAccounts;
        private TimeServerTranslation _translation;
        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();

            _timeServerAccounts = new ObservableCollection<TimeServerAccount>();
            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.TimeServerAccounts = _timeServerAccounts;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            _translation = new TimeServerTranslation();
            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _timeServerAccountAddCommand = new TimeServerAccountAddCommand(_interactionRequest, _accountsProvider, translationUpdater);
        }

        [Test]
        public void CanExcute_IsAlwaysTrue()
        {
            Assert.IsTrue(_timeServerAccountAddCommand.CanExecute(null));
        }

        [Test]
        public void AddAccount_InteractionRequestRaisesTimeServerAccountInteraction()
        {
            _timeServerAccountAddCommand.Execute(null);

            _interactionRequest.AssertWasRaised<TimeServerAccountInteraction>();
        }

        [Test]
        public void AddAccount_RaisedInteractionHasCorrectTitle()
        {
            _timeServerAccountAddCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<TimeServerAccountInteraction>();
            Assert.AreEqual(_translation.AddTimeServerAccount, interaction.Title, "Wrong Title for TimeServerAccountInteraction");
        }

        [Test]
        public void AddAccount_RaisedInteractionContainsAccountWithID()
        {
            _timeServerAccountAddCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<TimeServerAccountInteraction>();
            Assert.IsFalse(string.IsNullOrWhiteSpace(interaction.TimeServerAccount.AccountId), "Raised interaction contains account without ID");
        }

        [Test]
        public void AddAccount_UserCancelsInteraction_NoAccountGetsAdded()
        {
            _interactionRequest.RegisterInteractionHandler<TimeServerAccountInteraction>(i =>
            {
                i.Success = false; //User cancels
            });

            _timeServerAccountAddCommand.Execute(null);

            Assert.IsEmpty(_timeServerAccounts);
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_AccountGetsAdded()
        {
            _interactionRequest.RegisterInteractionHandler<TimeServerAccountInteraction>(i =>
            {
                i.Success = true; //User applies
                i.TimeServerAccount.UserName = "Some value is set in interaction";
            });

            _timeServerAccountAddCommand.Execute(null);

            Assert.AreEqual(1, _timeServerAccounts.Count);
            Assert.AreEqual("Some value is set in interaction", _timeServerAccounts.First().UserName);
        }

        [Test]
        public void AddAccount_UserCancelsInteraction_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _timeServerAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;

            _interactionRequest.RegisterInteractionHandler<TimeServerAccountInteraction>(i =>
            {
                i.Success = false; // User cancels
            });

            _timeServerAccountAddCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _timeServerAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;

            _interactionRequest.RegisterInteractionHandler<TimeServerAccountInteraction>(i =>
            {
                i.Success = true; // User applies
            });

            _timeServerAccountAddCommand.Execute(null);

            Assert.IsTrue(wasCalled);
        }
    }
}
