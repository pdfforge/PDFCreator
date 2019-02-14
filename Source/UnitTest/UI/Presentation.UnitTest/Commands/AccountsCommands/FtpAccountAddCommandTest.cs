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
    public class FtpAccountAddCommandTest
    {
        private IWaitableCommand _ftpAccountAddCommand;
        private UnitTestInteractionRequest _interactionRequest;
        private ObservableCollection<FtpAccount> _ftpAccounts;
        private FtpActionTranslation _translation;
        private ICurrentSettings<Accounts> _accountsProvider;

        [SetUp]
        public void SetUp()
        {
            _interactionRequest = new UnitTestInteractionRequest();

            _ftpAccounts = new ObservableCollection<FtpAccount>();
            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.Accounts.FtpAccounts = _ftpAccounts;
            _accountsProvider = Substitute.For<ICurrentSettings<Accounts>>();
            _accountsProvider.Settings.Returns(settings.ApplicationSettings.Accounts);

            _translation = new FtpActionTranslation();
            var translationUpdater = new TranslationUpdater(new TranslationFactory(), new ThreadManager());

            _ftpAccountAddCommand = new FtpAccountAddCommand(_interactionRequest, _accountsProvider, translationUpdater);
        }

        [Test]
        public void CanExcute_IsAlwaysTrue()
        {
            Assert.IsTrue(_ftpAccountAddCommand.CanExecute(null));
        }

        [Test]
        public void AddAccount_InteractionRequestRaisesFtpAccountInteraction()
        {
            _ftpAccountAddCommand.Execute(null);

            _interactionRequest.AssertWasRaised<FtpAccountInteraction>();
        }

        [Test]
        public void AddAccount_RaisedInteractionHasCorrectTitle()
        {
            _ftpAccountAddCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<FtpAccountInteraction>();
            Assert.AreEqual(_translation.AddFtpAccount, interaction.Title, "Wrong Title for FtpAccountInteraction");
        }

        [Test]
        public void AddAccount_RaisedInteractionContainsAccountWithID()
        {
            _ftpAccountAddCommand.Execute(null);

            var interaction = _interactionRequest.AssertWasRaised<FtpAccountInteraction>();
            Assert.IsFalse(string.IsNullOrWhiteSpace(interaction.FtpAccount.AccountId), "Raised interaction contains account without ID");
        }

        [Test]
        public void AddAccount_UserCancelsInteraction_NoAccountGetsAdded_IsDoneCalledWithCancel()
        {
            var wasCalled = false;
            _ftpAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Cancel;

            _interactionRequest.RegisterInteractionHandler<FtpAccountInteraction>(i =>
            {
                i.Success = false; //User cancels
            });

            _ftpAccountAddCommand.Execute(null);

            Assert.IsEmpty(_ftpAccounts);

            Assert.IsTrue(wasCalled, "ResponseStatus not Cancel");
        }

        [Test]
        public void AddAccount_UserAppliesInteraction_AccountGetsAdded_IsDoneCalledWithSuccess()
        {
            var wasCalled = false;
            _ftpAccountAddCommand.IsDone += (sender, args) => wasCalled = args.ResponseStatus == ResponseStatus.Success;

            _interactionRequest.RegisterInteractionHandler<FtpAccountInteraction>(i =>
            {
                i.Success = true; //User applies
                i.FtpAccount.UserName = "Some value set in Interaction";
            });

            _ftpAccountAddCommand.Execute(null);

            Assert.AreEqual(1, _ftpAccounts.Count);
            Assert.AreEqual("Some value set in Interaction", _ftpAccounts.First().UserName);

            Assert.IsTrue(wasCalled, "ResponseStatus not Success");
        }
    }
}
