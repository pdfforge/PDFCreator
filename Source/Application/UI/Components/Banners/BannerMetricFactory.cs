using pdfforge.PDFCreator.Utilities;
using pdfforge.UsageStatistics;

namespace Banners
{
    public interface IBannerMetricFactory
    {
        BannerMetric BuildMetric(BannerDefinition banner, BannerMetricType type);
    }

    public class BannerMetricFactory : IBannerMetricFactory
    {
        private readonly ApplicationNameProvider _applicationNameProvider;
        private readonly IVersionHelper _versionHelper;
        private readonly IUsageMetricFactory _usageMetricFactory;

        public BannerMetricFactory(ApplicationNameProvider applicationNameProvider, IVersionHelper versionHelper, IUsageMetricFactory usageMetricFactory)
        {
            _applicationNameProvider = applicationNameProvider;
            _versionHelper = versionHelper;
            _usageMetricFactory = usageMetricFactory;
        }

        public BannerMetric BuildMetric(BannerDefinition banner, BannerMetricType type)
        {
            var metric = _usageMetricFactory.CreateMetric<BannerMetric>();
            metric.Activity = type;
            metric.BundleId = banner.BundleId;
            metric.Campaign = banner.Campaign;
            metric.BundleVersion = banner.Version;

            return metric;
        }
    }
}
