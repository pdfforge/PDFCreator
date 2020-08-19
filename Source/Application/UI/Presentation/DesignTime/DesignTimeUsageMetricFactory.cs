using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUsageMetricFactory : IUsageMetricFactory
    {
        private readonly UsageMetricFactory _factory = new UsageMetricFactory(new UsageStatisticsOptions("", "pdfcreator", "1.2.3"));

        public T CreateMetric<T>() where T : UsageMetricBase, new()
        {
            return _factory.CreateMetric<T>();
        }
    }
}
