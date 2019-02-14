using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public interface IPdfCreatorUsageStatisticsManager
    {
        void SendUsageStatistics(TimeSpan duration, Job job, string status);
    }
}
