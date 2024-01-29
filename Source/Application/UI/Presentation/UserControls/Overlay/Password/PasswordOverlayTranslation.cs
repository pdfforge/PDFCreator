using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password
{
    public class PasswordOverlayTranslation : ITranslatable
    {
        public string CancelButtonContent { get; private set; } = "_Cancel";
        public string OkButtonContent { get; private set; } = "_Ok";
        public string PasswordHintText { get; private set; } = "Leave password empty to get a request during the print job (password will not be saved).";
        public string RemoveButtonContent { get; private set; } = "_Remove";
        public string ReenterPassword { get; set; } = "Invalid Credentials";
        private string InvalidPasswordMessage { get; set; } = "Invalid credentials for '{0}'. Please try retyping your password.";

        public string FormatInvalidPasswordMessage(string actionName)
        {
            return string.Format(InvalidPasswordMessage, actionName);
        }
    }
}
