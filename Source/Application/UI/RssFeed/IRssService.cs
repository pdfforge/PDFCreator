using System.Collections.Generic;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.RssFeed
{
    public interface IRssService
    {
        Task<List<FeedItem>> FetchFeedAsync(string url);
    }
}