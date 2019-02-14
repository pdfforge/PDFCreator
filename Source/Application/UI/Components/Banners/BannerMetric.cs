using pdfforge.PDFCreator.Core.UsageStatistics;

namespace Banners
{
    public class BannerMetric : IUsageMetric
    {
        public BannerMetric(string product, string machineId, string version, string bundleId, int bundleVersion, BannerMetricType type, string campaign)
        {
            Product = product;
            MachineId = machineId;
            Version = version;
            BundleId = bundleId;
            BundleVersion = bundleVersion;
            Activity = type;
            Campaign = campaign;
        }

        public string EventName => "Banner";
        public string Product { get; }
        public string MachineId { get; }
        public string Version { get; }
        public string BundleId { get; }
        public int BundleVersion { get; }
        public string Campaign { get; }
        public BannerMetricType Activity { get; }
    }

    public enum BannerMetricType
    {
        Impression,
        Click
    }
}
