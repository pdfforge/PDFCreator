namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class ServiceUsageStatisticsMetric : IUsageMetric
    {
        public string EventName => "ServiceMetric";

        public string Product { get; set; }
        public string MachineId { get; set; }
        public string Version { get; set; }
        public int TotalDocuments { get; set; }
        public int TotalUsers { get; set; }
        public string OperatingSystem { get; set; }
        public long ServiceUptime { get; set; }
    }
}
