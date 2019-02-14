using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.Utilities.Threading;
using Ploeh.AutoFixture;
using Translatable;

namespace Presentation.UnitTest.UserControls
{
    [TestFixture]
    public class FtpPasswordViewModelUnitTest
    {
        private Job _job;
        private FtpAccount _ftpAccount;
        private IFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _ftpAccount = new FtpAccount() { AccountId = "FTPAccountID1" };

            var accounts = new Accounts();
            accounts.FtpAccounts.Add(_ftpAccount);

            _job = new Job(null, new ConversionProfile(), accounts);

            _job.Profile.Ftp.AccountId = _ftpAccount.AccountId;

            _fixture = new Fixture();
        }

        private FtpJobStepPasswordViewModel BuildViewModel()
        {
            return new FtpJobStepPasswordViewModel(new TranslationUpdater(new TranslationFactory(), new ThreadManager()));
        }

        [Test]
        public void OkButton_Execute_RaisesFinishEvent()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            var wasRaised = false;
            viewModel.StepFinished += (sender, args) => wasRaised = true;

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.Execute(null);

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void OkButton_Execute_WhenPasswordWasEntered_AppliesPasswordToJobPasswords()
        {
            var expectedPassword = _fixture.Create<string>();

            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);

            viewModel.Password = expectedPassword;
            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.Execute(null);

            Assert.AreEqual(_job.Passwords.FtpPassword, expectedPassword);
        }

        [Test]
        public void OkButton_CanExecute_PasswordIsEmpty_ReturnsFalse()
        {
            var viewModel = BuildViewModel();

            viewModel.Password = "";

            Assert.IsFalse(viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkButton_CanExecute_PasswordIsNotEmpty_ReturnsTrue()
        {
            var viewModel = BuildViewModel();

            viewModel.Password = "Not empty";

            Assert.IsTrue(viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void SkipCommand_OnExecute_RaisesFinishEvent()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            var wasRaised = false;
            viewModel.StepFinished += (sender, args) => wasRaised = true;

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.SkipCommand.Execute(null);

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void SkipCommand_OnExecute_ResponseIsSkip_PasswordIsEmpty_FtpActionGetsDisabled()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            _job.Profile.Ftp.Enabled = true;

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.SkipCommand.Execute(null);

            Assert.AreEqual("", viewModel.Password, "Password was not empty");
            Assert.IsFalse(_job.Profile.Ftp.Enabled, "FtpAction was not disabled");
        }

        [Test]
        public void CancelCommand_OnExecute_RaisesFinishEventAndThrowsAbortWorkflowException()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            var wasRaised = false;

            viewModel.StepFinished += (sender, args) => wasRaised = true;

            Assert.Throws<AbortWorkflowException>(() => viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.CancelCommand.Execute(null));
            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void SetPassword_RaisesOkButtonCanExecuteChanged()
        {
            var viewModel = BuildViewModel();
            var wasRaised = false;
            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            viewModel.Password = "Some Password";

            Assert.IsTrue(wasRaised);
        }
    }
}
