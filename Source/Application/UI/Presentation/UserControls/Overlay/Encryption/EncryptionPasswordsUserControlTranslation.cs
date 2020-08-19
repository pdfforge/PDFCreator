using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Encryption
{
    public class EncryptionPasswordsUserControlTranslation : ITranslatable
    {
        public string CancelButtonContent { get; private set; } = "Cancel";
        public string OkButtonContent { get; private set; } = "OK";
        public string OwnerPasswordLabelContent { get; private set; } = "Owner password (for editing):";
        public string OwnerPasswordHint { get; private set; } = "An owner password is required to set the user passwords.";
        public string LeavePasswordEmptyHint { get; private set; } = "Leave one or both passwords empty to get a request during the print job (passwords will not be saved).";
        public string RemoveButtonContent { get; private set; } = "Remove";
        public string SkipButtonContent { get; private set; } = "Skip";
        public string Title { get; private set; } = "Encryption Passwords";
        public string UserPasswordLabelContent { get; private set; } = "User password (for opening):";
        public string PasswordIndicatorText { get; private set; } = "Password strength:";
    }
}
