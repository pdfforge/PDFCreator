using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Core.UsageStatistics;
using System;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUsageStatisticsManager : IUsageStatisticsManager
    {
        public Task SendUsageStatistics(TimeSpan duration, Job job, string status)
        {
            throw new NotImplementedException();
        }

        public Task SendServiceStatistics(TimeSpan duration)
        {
            throw new NotImplementedException();
        }

        public bool EnableUsageStatistics { get; set; }
    }
}
