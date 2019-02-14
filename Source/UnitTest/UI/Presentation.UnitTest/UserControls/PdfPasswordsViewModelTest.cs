using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
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
    public class PdfPasswordsViewModelTest
    {
        private Job _job;
        private Security _securitySettings;
        private bool _finishEventWasCalled;

        [SetUp]
        public void Setup()
        {
            _finishEventWasCalled = false;
            _job = new Job(new JobInfo(), new ConversionProfile(), new Accounts());
            _securitySettings = _job.Profile.PdfSettings.Security;
        }

        private PdfJobStepPasswordViewModel BuildViewModel(bool askOwnerPassword = true, bool askUserPassword = false)
        {
            if (askOwnerPassword)
                _securitySettings.OwnerPassword = "owner";

            if (askUserPassword)
            {
                _securitySettings.UserPassword = "user";
                _securitySettings.RequireUserPassword = true;
            }

            var viewModel = new PdfJobStepPasswordViewModel(new TranslationUpdater(new TranslationFactory(), new ThreadManager()));
            viewModel.AskOwnerPassword = askOwnerPassword;
            viewModel.AskUserPassword = askUserPassword;

            viewModel.StepFinished += (sender, args) => _finishEventWasCalled = true;

            return viewModel;
        }

        [Test]
        public void OkCommand_CanExecute_WhenNothingIsSet_ReturnsFalse()
        {
            var viewModel = BuildViewModel();
            Assert.IsFalse(viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_CanExecute_OwnerPasswordIsRequiredAndNotSet_IsFalse()
        {
            var viewModel = BuildViewModel(true, false);
            Assert.IsFalse(viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_CanExecuteUserPasswordIsRequiredAndNotSet_IsFalse()
        {
            var viewModel = BuildViewModel(false, true);
            _securitySettings.UserPassword = "";
            Assert.IsFalse(viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_CanExecute_RequiredPasswordsAreSet_IsTrue()
        {
            var viewModel = BuildViewModel(false, true);
            viewModel.OwnerPassword = "owner password";
            viewModel.UserPassword = "MyPassword";
            Assert.IsTrue(viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_CanExecute_UserPasswordIsRequiredAndNotSet_IsFalse()
        {
            var viewModel = BuildViewModel(false, true);
            Assert.IsFalse(viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_OnExecute_SetsJobPasswords()
        {
            var viewModel = BuildViewModel(true, true);

            viewModel.ExecuteWorkflowStep(_job);
            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.Execute(null);

            Assert.AreEqual(_securitySettings.OwnerPassword, _job.Passwords.PdfOwnerPassword);
            Assert.AreEqual(_securitySettings.UserPassword, _job.Passwords.PdfUserPassword);
        }

        [Test]
        public void OnInteractionSet_SetsPasswordsInView()
        {
            var viewModel = BuildViewModel(true, true);

            var actionWasCalled = false;
            viewModel.SetPasswordsInUi = (x, y) => actionWasCalled = true;

            viewModel.ExecuteWorkflowStep(_job);
            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.Execute(null);

            Assert.IsTrue(actionWasCalled);
        }

        [Test]
        public void SettingOwnerPassword_CallsCanExecuteChanged()
        {
            var viewModel = BuildViewModel(true, true);
            var canExecuteChanged = false;
            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecuteChanged += (sender, args) => canExecuteChanged = true;

            viewModel.OwnerPassword = "MyPassword";

            Assert.IsTrue(canExecuteChanged);
        }

        [Test]
        public void SettingUserPassword_CallsCanExecuteChanged()
        {
            var viewModel = BuildViewModel(true, true);
            var canExecuteChanged = false;
            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecuteChanged += (sender, args) => canExecuteChanged = true;

            viewModel.UserPassword = "MyPassword";

            Assert.IsTrue(canExecuteChanged);
        }

        [Test]
        public void SkipCommand_OnExecute_CompletesInteraction()
        {
            var viewModel = BuildViewModel(true, true);

            viewModel.ExecuteWorkflowStep(_job);
            viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.SkipCommand.Execute(null);

            Assert.IsTrue(_finishEventWasCalled);
        }

        [Test]
        public void CancelCommand_OnExecute_CompletesInteraction()
        {
            var viewModel = BuildViewModel(true, true);

            viewModel.ExecuteWorkflowStep(_job);

            Assert.Throws<AbortWorkflowException>(() => viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.CancelCommand.Execute(null));

            Assert.IsTrue(_finishEventWasCalled);
        }

        [Test]
        public void AfterInitializing_AskPasswordPropertiesGetSet()
        {
            _securitySettings.OwnerPassword = "owner";
            _securitySettings.UserPassword = "user";
            var viewModel = BuildViewModel();

            viewModel.ExecuteWorkflowStep(_job);

            Assert.IsFalse(viewModel.AskOwnerPassword);
            Assert.IsFalse(viewModel.AskUserPassword);
        }

        [Test]
        public void AfterSettingJob__AskPasswordPropertiesGetSet()
        {
            _securitySettings.Enabled = true;
            _securitySettings.OwnerPassword = "";
            var viewModel = BuildViewModel(false, false);

            viewModel.ExecuteWorkflowStep(_job);

            Assert.IsTrue(viewModel.AskOwnerPassword);
            Assert.IsFalse(viewModel.AskUserPassword);
        }
    }
}
