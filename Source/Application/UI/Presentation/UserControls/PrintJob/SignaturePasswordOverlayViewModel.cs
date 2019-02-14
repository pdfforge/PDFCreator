using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class SignaturePasswordOverlayViewModel : PasswordOverlayViewModelBase<SignaturePasswordInteraction, SignaturePasswordTranslation>
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

        public SignaturePasswordOverlayViewModel(ITranslationUpdater translationUpdater, ISignaturePasswordCheck signaturePasswordCheck) : base(translationUpdater)
        {
            _signaturePasswordCheck = signaturePasswordCheck;
        }

        public override string Title => Translation.SiganturePasswordTitle;

        public override bool CanExecuteHook()
        {
            if (!base.CanExecuteHook())
                return false;

            if (string.IsNullOrEmpty(CertificatePath))
                return false;

            return _signaturePasswordCheck.IsValidPassword(CertificatePath, Password);
        }

        protected override void HandleInteractionObjectChanged()
        {
            base.HandleInteractionObjectChanged();
            CertificatePath = Interaction.CertificateFile;
        }
    }

    public class DesignTimeSignaturePasswordOverlayViewModel : SignaturePasswordOverlayViewModel
    {
        public DesignTimeSignaturePasswordOverlayViewModel() : base(new DesignTimeTranslationUpdater(), null)
        {
        }
    }
}
