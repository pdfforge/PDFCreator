using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public interface IHttpHandler
    {
        HttpResponseMessage Post(Uri uri, HttpContent content);

        Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content);
    }

    public class HttpClientWrapper : IHttpHandler
    {
        private readonly HttpClient _client = new HttpClient();

        public HttpResponseMessage Post(Uri uri, HttpContent content)
        {
            return PostAsync(uri, content).Result;
        }

        public async Task<HttpResponseMessage> PostAsync(Uri uri, HttpContent content)
        {
            try
            {
                return await _client.PostAsync(uri, content);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }
    }
}
