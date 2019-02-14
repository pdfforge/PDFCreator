using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Banners
{
    public interface IDownloader
    {
        Task<Stream> OpenReadTaskAsync(string url);

        Task<string> DownloadStringTaskAsync(string url);
    }

    public class WebClientDownloader : IDownloader
    {
        private readonly WebClient _webClient;

        public WebClientDownloader()
        {
            _webClient = new WebClient();
        }

        public Task<Stream> OpenReadTaskAsync(string url)
        {
            return _webClient.OpenReadTaskAsync(url);
        }

        public Task<string> DownloadStringTaskAsync(string url)
        {
            return _webClient.DownloadStringTaskAsync(url);
        }
    }
}
