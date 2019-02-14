using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.Utilities;

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
        private readonly IMachineIdGenerator _machineIdGenerator;

        private string _machineId;

        private string MachineId => _machineId ?? (_machineId = _machineIdGenerator.GetMachineId());

        public BannerMetricFactory(ApplicationNameProvider applicationNameProvider, IVersionHelper versionHelper, IMachineIdGenerator machineIdGenerator)
        {
            _applicationNameProvider = applicationNameProvider;
            _versionHelper = versionHelper;
            _machineIdGenerator = machineIdGenerator;
        }

        public BannerMetric BuildMetric(BannerDefinition banner, BannerMetricType type)
        {
            var productName = _applicationNameProvider.ApplicationNameWithEdition.ToLowerInvariant().Replace(" ", "_");
            var metric = new BannerMetric(productName, MachineId, _versionHelper.FormatWithThreeDigits(), banner.BundleId, banner.Version, type, banner.Campaign);

            return metric;
        }
    }
}
