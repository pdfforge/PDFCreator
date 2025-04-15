using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Services.Download
{
    public interface IDownloader
    {
        Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default);
        Task DownloadFileAsync(string url, string targetFile, CancellationToken cancellationToken, Action<int> progressAction = null);
    }

    public class HttpClientDownloader : IDownloader
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpClientDownloader(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetStringAsync(string url, CancellationToken cancellationToken = default)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            return await httpClient.GetStringAsync(url, cancellationToken);
        }

        public async Task DownloadFileAsync(string url, string targetFile, CancellationToken cancellationToken, Action<int> progressAction = null)
        {
            using var client = _httpClientFactory.CreateClient();
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            var totalBytes = response.Content.Headers.ContentLength ?? -1L;

            await using var fileStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write, FileShare.None);
            await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            var buffer = new byte[8192]; // standard buffer size for file downloading to balance memory usage and performance.
            var totalBytesRead = 0L;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                totalBytesRead += bytesRead;

                if (totalBytes != -1) // handle unknown content length
                {
                    var progressPercentage = (int)((totalBytesRead * 100) / totalBytes);
                    progressAction?.Invoke(progressPercentage);
                }
            }
        }
    }
}
