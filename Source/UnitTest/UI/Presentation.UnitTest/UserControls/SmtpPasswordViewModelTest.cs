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
    public class SmtpPasswordViewModelTest
    {
        private Job _job;
        private SmtpAccount _smtpAccout;

        [SetUp]
        public void Setup()
        {
            _smtpAccout = new SmtpAccount() { AccountId = "SmtpAccountID1" };

            var accounts = new Accounts();
            accounts.SmtpAccounts.Add(_smtpAccout);

            _job = new Job(null, new ConversionProfile(), accounts);
            _job.Profile.EmailSmtpSettings.AccountId = _smtpAccout.AccountId;
        }

        private SmtpJobStepPasswordViewModel BuildViewModel()
        {
            return new SmtpJobStepPasswordViewModel(new TranslationUpdater(new TranslationFactory(), new ThreadManager()));
        }

        [Test]
        public void SmtPasswordViewModel_CheckClass_ClassNotNull()
        {
            SmtpJobStepPasswordViewModel viewModel = new SmtpJobStepPasswordViewModel(new TranslationUpdater(new TranslationFactory(), new ThreadManager()));

            Assert.NotNull(viewModel);
        }

        [Test]
        public void OKCommand_OnExecute_RaisesFinishEvent()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            var wasRaised = false;
            viewModel.StepFinished += (sender, args) => wasRaised = true;

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.Execute(null);

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void OKCommand_OnExecute_SetsPasswordInJobPasswords()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            viewModel.Password = "Some User Password";

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.Execute(null);

            Assert.AreEqual(viewModel.Password, _job.Passwords.SmtpPassword);
        }

        [Test]
        public void OkCommand_CanExecute_PasswordNotEmpty_ReturnsTrue()
        {
            var viewModel = BuildViewModel();
            viewModel.Password = "Some Password";

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
        public void SkipCommand_OnExecute_SmtpActionGetsDisabled()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            _job.Profile.EmailSmtpSettings.Enabled = true;

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.SkipCommand.Execute(null);

            Assert.IsFalse(_job.Profile.EmailSmtpSettings.Enabled, "SmtpAction was not disabled");
        }

        [Test]
        public void SkipCommand_OnExecute_JobSmtpPasswordIsEmptyString()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            viewModel.Password = "Not Empty Password";

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.SkipCommand.Execute(null);

            Assert.AreEqual("", _job.Passwords.SmtpPassword);
        }

        [Test]
        public void CancelCommand_OnExecute_RaisesFinishedEventAndThrowsAbortWorkflowException_JobSmtpPasswordIsEmptyString()
        {
            var viewModel = BuildViewModel();
            viewModel.ExecuteWorkflowStep(_job);
            viewModel.Password = "Not Empty Password";
            var wasRaised = false;

            viewModel.StepFinished += (sender, args) => wasRaised = true;

            Assert.Throws<AbortWorkflowException>(() => viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.CancelCommand.Execute(null));
            Assert.IsTrue(wasRaised);
            Assert.AreEqual("", _job.Passwords.SmtpPassword);
        }

        [Test]
        public void SetPassword_RaisesOkCommandCanExecuteChanged()
        {
            var viewModel = BuildViewModel();
            var wasRaised = false;

            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            viewModel.Password = "Some Password";

            Assert.IsTrue(wasRaised);
        }
    }
}
