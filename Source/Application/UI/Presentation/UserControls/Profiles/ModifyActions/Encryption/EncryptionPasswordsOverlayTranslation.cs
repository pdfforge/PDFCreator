using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Encryption
{
    public class EncryptionPasswordsOverlayTranslation : ITranslatable
    {
        public string CancelButtonContent { get; private set; } = "Cancel";
        public string OkButtonContent { get; private set; } = "Ok";
        public string OwnerPasswordLabelContent { get; private set; } = "Owner password (for editing):";
        public string OwnerPasswordsHint { get; private set; } = "Note: An owner password is required to set the user password.";
        public string PasswordHintText { get; private set; } = "Leave password empty to get a request during the print job (password will not be saved).";
        public string RemoveButtonContent { get; private set; } = "Remove";
        public string SkipButtonContent { get; private set; } = "Skip";
        public string Title { get; private set; } = "Encryption Passwords";
        public string UserPasswordLabelContent { get; private set; } = "User password (for opening):";
        public string PasswordIndicatorText { get; private set; } = "Password strength:";
    }
}
