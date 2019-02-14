namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public interface IUsageMetric
    {
        string Product { get; }
        string MachineId { get; }
        string Version { get; }
        string EventName { get; }
    }
}
