using System;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using pdfforge.DataStorage;
using pdfforge.PDFCreator.Core.UsageStatistics.Properties;
using pdfforge.PDFCreator.Utilities;
using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class AvqUsageStatisticsSender : IUsageStatisticsSender
    {
        private readonly UsageStatisticsOptions _usageStatisticsOptions;
        private readonly IUsageStatisticsJsonSerializer _usageStatisticsJsonSerializer;
        private readonly int _timeout = 10;

        public AvqUsageStatisticsSender(UsageStatisticsOptions usageStatisticsOptions, IUsageStatisticsJsonSerializer usageStatisticsJsonSerializer)
        {
            _usageStatisticsOptions = usageStatisticsOptions;
            _usageStatisticsJsonSerializer = usageStatisticsJsonSerializer;
        }

        public async Task SendAsync(IUsageMetric usageMetric)
        {
            var response = await DoSendAsync(usageMetric);
        }

        public void Send(IUsageMetric usageMetric)
        {
            var response = DoSendAsync(usageMetric).GetAwaiter().GetResult();
        }

        private async Task<HttpResponseMessage> DoSendAsync(IUsageMetric usageMetric)
        {
            if (!_usageStatisticsOptions.IsEnabled)
                return new HttpResponseMessage(HttpStatusCode.OK);

            try
            {
                var uri = new Uri(Urls.AvqUsageStatisticsStagingEndpointUrl);
                var key = Data.Decrypt(Resources.AvqTrackingKey);
                var json = _usageStatisticsJsonSerializer.Serialize(usageMetric);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", key);

                    client.Timeout = TimeSpan.FromSeconds(_timeout);
                    return await client.PostAsync(uri, content);
                }
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}