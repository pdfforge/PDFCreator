using System.Windows.Input;
using SystemInterface.IO;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using pdfforge.PDFCreator.Utilities;
using Prism.Commands;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    public class SignaturePasswordOverlayViewModel : OverlayViewModelBase<SignaturePasswordInteraction, SignatureTranslation>
    {
        public override string Title => Translation.SignaturePasswordButton;

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
                RaisePropertyChanged(nameof(PasswordIsValid));
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public string CertificateFile => PathSafe.GetFileName(CertificatePath);

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                RaisePropertyChanged(nameof(Password));
                RaisePropertyChanged(nameof(PasswordIsValid));
                OkCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand OkCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public SignaturePasswordOverlayViewModel(ITranslationUpdater translationUpdater, ISignaturePasswordCheck signaturePasswordCheck) : base(translationUpdater)
        {
            _signaturePasswordCheck = signaturePasswordCheck;

            OkCommand = new DelegateCommand(OkExecute, () => PasswordIsValid);
            CancelCommand = new DelegateCommand(CancelExecute);
            RemoveCommand = new DelegateCommand(RemoveExecute);
        }

        public bool PasswordIsValid {
            get
            {
                if (string.IsNullOrEmpty(Password))
                    return false;

                if (string.IsNullOrEmpty(CertificatePath))
                    return false;

                return _signaturePasswordCheck.IsValidPassword(CertificatePath, Password);
            }
        }

        private void OkExecute()
        {
            Interaction.Password = Password;
            Interaction.Result = PasswordResult.StorePassword;
            FinishInteraction();
        }

        private void CancelExecute()
        {
            Interaction.Result = PasswordResult.Cancel;
            FinishInteraction();
        }

        private void RemoveExecute()
        {
            Interaction.Password = "";
            Interaction.Result = PasswordResult.RemovePassword;
            FinishInteraction();
        }

        protected override void HandleInteractionObjectChanged()
        {
            Password = Interaction.Password;
            CertificatePath = Interaction.CertificatePath;
        }
    }
}
