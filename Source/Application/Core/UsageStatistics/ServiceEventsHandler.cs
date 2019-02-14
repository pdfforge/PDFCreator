using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.Services.JobEvents;
using System;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class ServiceEventsHandler : IJobEventsHandler, IServiceEventsHandler
    {
        private readonly IUsageStatisticsManager _usageStatisticsManager;

        public ServiceEventsHandler(IUsageStatisticsManager usageStatisticsManager)
        {
            _usageStatisticsManager = usageStatisticsManager;
        }

        public void HandleJobStarted(Job job, string currentThreadName)
        {
        }

        public void HandleJobCompleted(Job job, TimeSpan duration)
        {
            _usageStatisticsManager.SendUsageStatistics(duration, job, "Success");
        }

        public void HandleJobFailed(Job job, TimeSpan duration, FailureReason reason)
        {
            _usageStatisticsManager.SendUsageStatistics(duration, job, reason.ToString());
        }

        public void HandleServiceStopped(TimeSpan serviceUptime)
        {
            _usageStatisticsManager.SendServiceStatistics(serviceUptime);
        }
    }
}
