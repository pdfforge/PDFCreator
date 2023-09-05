using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Core.Workflow.Exceptions
{
    public class BaseFailedJobHandler : IFailedJobHandler
    {
        private readonly INotificationService _notificationService;

        public BaseFailedJobHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public virtual void HandleFailedJob(Job job)
        {
            if (job.Profile.AutoSave.Enabled)
            {
                var documentName = job.JobInfo.Metadata.Title;
                var currentProfile = job.Profile;

                if (currentProfile.ShowAllNotifications || currentProfile.ShowOnlyErrorNotifications)
                    _notificationService?.ShowErrorNotification(documentName);
            }
        }
    }
}
