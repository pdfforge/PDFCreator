using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class ServiceUsageStatisticsMetric : UsageMetricBase
    {
        public override string EventName => "ServiceMetric";

        public int TotalDocuments { get; set; }
        public int TotalUsers { get; set; }
        public string OperatingSystem { get; set; }
        public long ServiceUptime { get; set; }
    }
}
