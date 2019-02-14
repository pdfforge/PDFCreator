using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class SignaturePasswordStepViewModel : JobStepPasswordViewModelBase<SignaturePasswordTranslation>
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

        public SignaturePasswordStepViewModel(ITranslationUpdater translationUpdater, ISignaturePasswordCheck signaturePasswordCheck) : base(translationUpdater, nameof(JobPasswords.PdfSignaturePassword))
        {
            _signaturePasswordCheck = signaturePasswordCheck;
        }

        protected override string GetCancelErrorMessage()
        {
            return "User cancelled in SignaturePasswordView";
        }

        public override void SkipHook()
        {
            Job.Profile.PdfSettings.Signature.Enabled = false;
        }

        protected override void ExecuteWorkflow()
        {
            CertificatePath = Job.Profile.PdfSettings.Signature.CertificateFile;
        }

        public override bool CanExecuteHook()
        {
            return base.CanExecuteHook() && _signaturePasswordCheck.IsValidPassword(CertificatePath, Password);
        }
    }
}
