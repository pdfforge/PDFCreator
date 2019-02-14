using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace Presentation.UnitTest.UserControls
{
    [TestFixture]
    public class HttpPasswordViewModelTest
    {
        private Job _job;
        private HttpAccount _httpAccount;

        [SetUp]
        public void Setup()
        {
            _httpAccount = new HttpAccount() { AccountId = "HttpAccountID1" };

            var accounts = new Accounts();
            accounts.HttpAccounts.Add(_httpAccount);

            _job = new Job(null, new ConversionProfile(), accounts);
            _job.Profile.HttpSettings.AccountId = _httpAccount.AccountId;
        }

        private HttpJobStepPasswordViewModel BuildViewModel()
        {
            return new HttpJobStepPasswordViewModel(new TranslationUpdater(new TranslationFactory(), new ThreadManager()));
        }

        [Test]
        public void HttpPasswordViewModel_HttpPasswordViewModelIsNotNull()
        {
            var viewModel = new HttpJobStepPasswordViewModel(new TranslationUpdater(new TranslationFactory(), new ThreadManager()));
            Assert.IsNotNull(viewModel);
        }

        [Test]
        public void OkCommand_OnExecute_RaisesFinishInteraction()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            var wasRaised = false;
            viewModel.StepFinished += (sender, args) => wasRaised = true;

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.Execute(null);

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void OkCommand_CanExecute_PasswordNotEmpty_ReturnsTrue()
        {
            var viewModel = BuildViewModel();

            viewModel.Password = "Password Not Empty";

            Assert.IsTrue(viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_CanExecute_PasswordIsEmpty_ReturnsFalse()
        {
            var viewModel = BuildViewModel();

            viewModel.Password = "";

            Assert.IsFalse(viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void SkipCommand_OnExecute_RaisesFinishInteraction()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            var wasRaised = false;

            viewModel.StepFinished += (sender, args) => wasRaised = true;
            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.SkipCommand.Execute(null);
            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void SkipCommand_OnExecute_JobHttpPasswordIsEmptyString()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            viewModel.Password = "Password Not Empty";

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.SkipCommand.Execute(null);

            Assert.AreEqual("", _job.Passwords.HttpPassword);
        }

        [Test]
        public void SkipCommand_OnExecute_HttpActionGetsDisabled()
        {
            _job.Profile.HttpSettings.Enabled = true;
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.SkipCommand.Execute(null);

            Assert.IsFalse(_job.Profile.HttpSettings.Enabled);
        }

        [Test]
        public void CancelCommand_OnExecute_RaisesFinishInteractionAndThrowsAbortWorkflowException_JobHttpPasswordIsEmptyString()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            viewModel.Password = "Not An Empty Password";
            var wasRaised = false;

            viewModel.StepFinished += (sender, args) => wasRaised = true;

            Assert.Throws<AbortWorkflowException>(() => viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.CancelCommand.Execute(null));
            Assert.IsTrue(wasRaised);
            Assert.AreEqual("", _job.Passwords.HttpPassword);
        }

        [Test]
        public void SetPassword_RaisesOkCommandCanExecuteChange()
        {
            var viewModel = BuildViewModel();
            var wasRaised = false;

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            viewModel.Password = "Some New Password";

            Assert.IsTrue(wasRaised);
        }
    }
}
