using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public abstract class SignaturePasswordViewModel : OverlayViewModelBase<SignaturePasswordInteraction, SignaturePasswordTranslation>
    {
        private readonly ISignaturePasswordCheck _signaturePasswordCheck;
        protected Job Job { get; set; }
        private string _password;

        public abstract PrintJobPasswordButtonViewModel PrintJobPasswordButtonViewModel { get; }

        public event EventHandler StepFinished;

        private string _certificateFile;

        protected string CertificatePath
        {
            get { return _certificateFile; }
            set
            {
                _certificateFile = value;
                RaisePropertyChanged(nameof(CertificatePath));
                RaisePropertyChanged(nameof(CertificateFile));
            }
        }

        public string CertificateFile => PathSafe.GetFileName(CertificatePath);

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                PrintJobPasswordButtonViewModel.OkCommand.RaiseCanExecuteChanged();
            }
        }

        public override string Title => Translation.SiganturePasswordTitle;

        protected SignaturePasswordViewModel(ITranslationUpdater translationUpdater, ISignaturePasswordCheck signaturePasswordCheck) : base(translationUpdater)
        {
            _signaturePasswordCheck = signaturePasswordCheck;
        }

        public void ExecuteWorkflowStep(Job job)
        {
            Job = job;
            CertificatePath = Job.Profile.PdfSettings.Signature.CertificateFile;
            Password = Job.Passwords.PdfSignaturePassword;
            RaisePropertyChanged(nameof(Password));
        }

        protected void CancelExecute()
        {
            Password = string.Empty;
            CloseView();

            CancelAction();
        }

        protected abstract void CancelAction();

        protected abstract void RemoveAction();

        protected abstract void OkAction();

        protected abstract void CloseAction();

        protected void SkipExecute()
        {
            Password = string.Empty;
            Job.Passwords.PdfSignaturePassword = Password;
            Job.Profile.PdfSettings.Signature.Enabled = false;

            CloseView();
        }

        protected void RemoveExecute()
        {
            Password = string.Empty;

            RemoveAction();

            CloseView();
        }

        protected void OkExecute()
        {
            OkAction();

            CloseView();
        }

        protected bool OkCanExecute()
        {
            if (Password == null)
                return false;

            return _signaturePasswordCheck.IsValidPassword(CertificatePath, Password);
        }

        protected override void HandleInteractionObjectChanged()
        {
            CertificatePath = Interaction.CertificateFile;
            Password = Interaction.Password;
            RaisePropertyChanged(nameof(Password));
        }

        private void CloseView()
        {
            StepFinished?.Invoke(this, EventArgs.Empty);
            CloseAction();
        }
    }
}
