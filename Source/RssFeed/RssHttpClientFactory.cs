using System.Net.Http;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.RssFeed
{
    public class RssHttpClientFactory : IRssHttpClientFactory
    {
        public IRssHttpClient CreateHttpClient()
        {
            return new RssHttpClientWrapper();
        }
    }

    public class RssHttpClientWrapper : HttpClient, IRssHttpClient
    {
    }

    public interface IRssHttpClient
    {
        Task<string> GetStringAsync(string requestUri);
    }
}
