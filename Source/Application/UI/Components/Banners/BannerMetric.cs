using pdfforge.UsageStatistics;

namespace Banners
{
    public class BannerMetric : UsageMetricBase
    {
        public override string EventName => "Banner";

        public string BundleId { get; set; }
        public int BundleVersion { get; set; }
        public string Campaign { get; set; }
        public BannerMetricType Activity { get; set; }
    }

    public enum BannerMetricType
    {
        Impression,
        Click
    }
}
