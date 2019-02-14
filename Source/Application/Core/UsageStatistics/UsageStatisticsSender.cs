using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using pdfforge.PDFCreator.Core.Controller;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public interface IUsageStatisticsSender
    {
        Task SendAsync(IUsageMetric metric);

        void Send(IUsageMetric metric);
    }

    public class UsageStatisticsSender : IUsageStatisticsSender
    {
        private readonly IHttpHandler _httpHandler;
        private readonly Uri _baseUri = new Uri(Urls.UsageStatisticsEndpointUrl);

        public UsageStatisticsSender(IHttpHandler httpHandler)
        {
            _httpHandler = httpHandler;
        }

        public async Task SendAsync(IUsageMetric usageMetric)
        {
            var json = Serialize(usageMetric);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await _httpHandler.PostAsync(new Uri(_baseUri, usageMetric.Product), content);
        }

        public void Send(IUsageMetric usageMetric)
        {
            var json = Serialize(usageMetric);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpHandler.Post(new Uri(_baseUri, usageMetric.Product), content);
        }

        private string Serialize(IUsageMetric usageMetric)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new SnakeCaseNamingStrategy(),
            };

            var settings = new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });

            return JsonConvert.SerializeObject(usageMetric, settings);
        }
    }
}
