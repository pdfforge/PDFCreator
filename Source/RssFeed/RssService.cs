using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace pdfforge.PDFCreator.UI.RssFeed
{
    public class RssService : IRssService
    {
        private readonly IRssHttpClientFactory _rssHttpClientFactory;

        public RssService(IRssHttpClientFactory rssHttpClientFactory)
        {
            _rssHttpClientFactory = rssHttpClientFactory;
        }

        public async Task<List<FeedItem>> FetchFeedAsync(string url)
        {
            try
            {
                var result = await _rssHttpClientFactory.CreateHttpClient().GetStringAsync(url);
                var document = XDocument.Parse(result);

                var feedItems = document.Descendants("item")
                    .Select(x => new FeedItem()
                    {
                        Title = (string)x.Element("title"),
                        Link = (string)x.Element("link"),
                        Description = (string)x.Element("description"),
                        PublishDate = DateTime.Parse((string)x.Element("pubDate"))
                    }).ToList();

                return feedItems;
            }
            catch
            {
                return new List<FeedItem>();
            }
        }
    }
}
