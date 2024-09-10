using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public interface IUsageStatisticsJsonSerializer
    {
        string Serialize(IUsageMetric usageMetric);
    }

    public  class UsageStatisticsJsonSerializer : IUsageStatisticsJsonSerializer
    {
        public string Serialize(IUsageMetric usageMetric)
        {
            var contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new SnakeCaseNamingStrategy(),
            };

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy() });

            return JsonConvert.SerializeObject(usageMetric, settings);
        }
    }
}
