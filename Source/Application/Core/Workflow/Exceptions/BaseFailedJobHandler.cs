using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow.Exceptions
{
    public class BaseFailedJobHandler : IFailedJobHandler
    {
        private readonly INotificationService _notificationService;

        public BaseFailedJobHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public virtual void HandleFailedJob(Job job, ErrorCode? errorCode)
        {
            if (!job.Profile.AutoSave.Enabled)
                return;
            var documentName = GetDocumentName(job);
            var currentProfile = job.Profile;

            if (currentProfile.ShowAllNotifications || currentProfile.ShowOnlyErrorNotifications)
                _notificationService?.ShowErrorNotification(documentName, errorCode);
        }

        private static string GetDocumentName(Job job)
        {
            var documentName = job.JobInfo.Metadata.Title;
            if (string.IsNullOrEmpty(documentName))
                documentName = PathSafe.GetFileName(job.JobInfo.OriginalFilePath);
            
            return documentName;
        }
    }
}
