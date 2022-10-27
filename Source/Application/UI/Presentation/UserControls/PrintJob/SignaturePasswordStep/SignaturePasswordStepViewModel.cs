using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class SignaturePasswordStepViewModel : JobStepPasswordViewModelBase<SignaturePasswordStepTranslation>
    {
        private readonly ISignaturePasswordCheck _signaturePasswordCheck;
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

        public SignaturePasswordStepViewModel(ITranslationUpdater translationUpdater, ISignaturePasswordCheck signaturePasswordCheck) : base(translationUpdater)
        {
            _signaturePasswordCheck = signaturePasswordCheck;
        }

        protected override void StorePasswordsInJobPasswords()
        {
            Job.Passwords.PdfSignaturePassword = Password;
        }

        protected override void DisableAction()
        {
            Job.Profile.PdfSettings.Signature.Enabled = false;
        }

        protected override void InitializeWorkflowStep()
        {
            CertificatePath = Job.Profile.PdfSettings.Signature.CertificateFile;
            Password = Job.Passwords.PdfSignaturePassword;
        }

        protected override bool ContinueCanExecute(object obj)
        {
            if (string.IsNullOrEmpty(Password))
                return false;
            return _signaturePasswordCheck.IsValidPassword(CertificatePath, Password);
        }
    }
}
