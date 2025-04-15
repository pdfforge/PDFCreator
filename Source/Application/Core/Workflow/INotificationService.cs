using pdfforge.PDFCreator.Conversion.Jobs;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface INotificationService
    {
        void ShowInfoNotification(string documentName, string documentPath);

        void ShowErrorNotification(string documentName, ErrorCode? errorCode);
    }

    public class DisabledNotificationService : INotificationService
    {
        public void ShowInfoNotification(string documentName, string documentPath)
        {
        }

        public void ShowErrorNotification(string documentName, ErrorCode? errorCode)
        {
        }
    }
}
