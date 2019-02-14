using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow.Exceptions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Threading;
using Ploeh.AutoFixture;
using Translatable;

namespace Presentation.UnitTest.UserControls
{
    [TestFixture]
    public class SignaturePasswordViewModelTest
    {
        private SignaturePasswordStepViewModel _viewModel;
        private Job _job;
        private ConversionProfile _conversionProfile;
        private ISignaturePasswordCheck _passwordCheck;
        private string _validSignaturePassword;
        private readonly IFixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            var certificateFile = _fixture.Create<string>();
            _validSignaturePassword = _fixture.Create<string>();

            _passwordCheck = Substitute.For<ISignaturePasswordCheck>();
            _passwordCheck.IsValidPassword(certificateFile, _validSignaturePassword).Returns(true);

            _conversionProfile = new ConversionProfile();
            _conversionProfile.PdfSettings.Signature.Enabled = true;
            _conversionProfile.PdfSettings.Signature.CertificateFile = certificateFile;

            _job = new Job(new JobInfo(), _conversionProfile, null);

            _viewModel = BuildViewModel();
            _viewModel.ExecuteWorkflowStep(_job);
        }

        private SignaturePasswordStepViewModel BuildViewModel()
        {
            return new SignaturePasswordStepViewModel(new TranslationUpdater(new TranslationFactory(), new ThreadManager()), _passwordCheck);
        }

        [Test]
        public void OkCommand_OnExecute_RaisesFinishEvent()
        {
            var wasRaised = false;
            _viewModel.StepFinished += (sender, args) => wasRaised = true;

            _viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.Execute(null);

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void OkCommand_OnExecute_SetsSignaturePasswordInJob()
        {
            _viewModel.Password = "password typed in by user";

            _viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.Execute(null);

            Assert.AreEqual(_viewModel.Password, _job.Passwords.PdfSignaturePassword);
        }

        [Test]
        public void OkCommand_CanExecute_PasswordByUserIsValid_ReturnsTrue()
        {
            _viewModel.Password = _validSignaturePassword;

            Assert.IsTrue(_viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void OkCommand_CanExecute_PasswordByUserIsInvalid_ReturnsFalse()
        {
            _viewModel.Password = "Not the valid password";

            Assert.IsFalse(_viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecute(null));
        }

        [Test]
        public void SkipCommand_Execute_RaisesFinishEvent()
        {
            var wasRaised = false;
            _viewModel.StepFinished += (sender, args) => wasRaised = true;

            _viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.SkipCommand.Execute(null);

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void SkipCommand_Execute_SignaturePasswordInJobIsEmpty()
        {
            _viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.SkipCommand.Execute(null);

            Assert.AreEqual("", _job.Passwords.PdfSignaturePassword);
        }

        [Test]
        public void SkipCommand_Execute_SigningGetsDisabled()
        {
            _viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.SkipCommand.Execute(null);

            Assert.IsFalse(_job.Profile.PdfSettings.Signature.Enabled);
        }

        [Test]
        public void CancelCommand_Execute_RaisesFinishEventAndThrowsAbortWorkflowException()
        {
            var wasRaised = false;
            _viewModel.StepFinished += (sender, args) => wasRaised = true;

            Assert.Throws<AbortWorkflowException>(() => _viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.CancelCommand.Execute(null));
            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void SetPassword_RaisesOkCommandCanExecuteChange()
        {
            var wasRaised = false;
            _viewModel.PasswordButtonController.PrintJobPasswordButtonViewModel.OkCommand.CanExecuteChanged += (sender, args) => wasRaised = true;

            _viewModel.Password = "Some New Password";

            Assert.IsTrue(wasRaised);
        }

        [Test]
        public void ExecuteWorkflowStep_SetsCertificateFileAndRaisesPropertyChanged()
        {
            var wasRaised = false;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_viewModel.CertificateFile))
                    wasRaised = true;
            };

            _viewModel.ExecuteWorkflowStep(_job);

            Assert.AreEqual(_conversionProfile.PdfSettings.Signature.CertificateFile, _viewModel.CertificateFile);
            Assert.IsTrue(wasRaised);
        }
    }
}
