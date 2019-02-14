using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using System;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class UsageStatisticsEventsHandler : IJobEventsHandler
    {
        private readonly IPdfCreatorUsageStatisticsManager _pdfCreatorUsageStatisticsManager;

        public UsageStatisticsEventsHandler(IPdfCreatorUsageStatisticsManager pdfCreatorUsageStatisticsManager)
        {
            _pdfCreatorUsageStatisticsManager = pdfCreatorUsageStatisticsManager;
        }

        public void HandleJobStarted(Job job, string currentThreadName)
        {
        }

        public void HandleJobCompleted(Job job, TimeSpan duration)
        {
            _pdfCreatorUsageStatisticsManager.SendUsageStatistics(duration, job, "Success");
        }

        public void HandleJobFailed(Job job, TimeSpan duration, FailureReason reason)
        {
            _pdfCreatorUsageStatisticsManager.SendUsageStatistics(duration, job, reason.ToString());
        }
    }
}
