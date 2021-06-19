using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class NotificationsLevelSettingsTranslation : ITranslatable
    {
        public string ShowAllNotifications { get; private set; } = "Show tray notification after converting a document";
        public string ShowOnlyErrorNotifications { get; private set; } = "Only show notification for errors";
    }
}
