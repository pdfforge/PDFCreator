using Translatable;

namespace pdfforge.PDFCreator.UI.Notifications
{
    public class NotificationTranslation : ITranslatable
    {
        private string SuccessNotificationMessage { get; set; } = "PDFCreator has successfully converted '{0}'.";
        private string ErrorNotificationMessage { get; set; } = "PDFCreator was not able to convert the document '{0}'.";

        public string SuccessTitle { get; protected set; } = "File successfully converted";
        public string ErrorTitle { get; protected set; } = "Error while converting";

        public string FormatSuccessNotificationMessage(string documentName)
        {
            return string.Format(SuccessNotificationMessage, documentName);
        }

        public string FormatErrorNotificationMessage(string documentName)
        {
            return string.Format(ErrorNotificationMessage, documentName);
        }
    }
}
