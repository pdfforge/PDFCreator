using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Encryption
{
    public class EncryptionPasswordsOverlayTranslation : ITranslatable
    {
        public string CancelButtonContent { get; private set; } = "Cancel";
        public string OkButtonContent { get; private set; } = "Ok";
        public string OwnerPasswordLabelContent { get; private set; } = "Owner password (for editing):";
        public string OwnerPasswordHint { get; private set; } = "An owner password is required for editing.";
        public string OwnerPasswordsHint { get; private set; } = "An owner password is required to set the user passwords.";
        public string LeavePasswordsEmptyHint { get; private set; } = "Leave one or both passwords empty to get a request during the print job (passwords will not be saved).";
        public string LeavePasswordEmptyHint { get; private set; } = "Leave the owner password empty to get a request during the print job (it will not be saved).";
        public string RemoveButtonContent { get; private set; } = "Remove";
        public string SkipButtonContent { get; private set; } = "Skip";
        public string Title { get; private set; } = "Encryption Passwords";
        public string UserPasswordLabelContent { get; private set; } = "User password (for opening):";
        public string PasswordIndicatorText { get; private set; } = "Password strength:";
    }
}
