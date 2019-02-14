using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.UI.RssFeed;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RssFeed.UnitTest
{
    [TestFixture]
    public class RssServiceUnitTest
    {
        private IRssHttpClientFactory _rssHttpClientFactory;
        private IRssHttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            _rssHttpClientFactory = Substitute.For<IRssHttpClientFactory>();
            _httpClient = Substitute.For<IRssHttpClient>();

            var settings = new PdfCreatorSettings();
            settings.ApplicationSettings.RssFeed = new pdfforge.PDFCreator.Conversion.Settings.RssFeed { Enable = true };
        }

        [Test]
        public async Task RssService_FetchesFeedAsync_WrongUrl_FeedItemsListIsEmpty()
        {
            var service = new RssService(_rssHttpClientFactory);

            var feedItems = await service.FetchFeedAsync("www.rongurl.nope");
            Assert.IsEmpty(feedItems);
        }

        [Test]
        public async Task RssService_FetchesFeedAsync_FeedItemsListHasEntries()
        {
            var expectedNumberOfItems = 2;
            var service = new RssService(_rssHttpClientFactory);

            var document = new XDocument();
            var xElement = new XElement("channel");
            document.Add(xElement);
            xElement.Add(
                CreateRssEntry("RSSTitle", "link@rss.com", "a useful description", "Fri, 06 Apr 2018 12:51:01 +0000"),
                CreateRssEntry("RSSTitle", "link@rss.com", "a useful description", "Fri, 06 Apr 2018 12:51:01 +0000")
            );

            _httpClient.GetStringAsync(Arg.Any<string>()).Returns(document.ToString());
            _rssHttpClientFactory.CreateHttpClient().Returns(_httpClient);

            var feedItems = await service.FetchFeedAsync(Urls.RssFeedUrl);
            Assert.IsNotEmpty(feedItems);
            Assert.AreEqual(expectedNumberOfItems, feedItems.Count);
        }

        private XElement CreateRssEntry(string title, string link, string description, string pubDate)
        {
            var element = new XElement("item");
            element.Add(new XElement("title").AddStringEntryExt(title));
            element.Add(new XElement("link").AddStringEntryExt(link));
            element.Add(new XElement("description").AddStringEntryExt(description));
            element.Add(new XElement("pubDate").AddStringEntryExt(pubDate));
            return element;
        }
    }

    public static class XElementExtension
    {
        public static XElement AddStringEntryExt(this XElement node, string value)
        {
            node.Add(value);
            return node;
        }
    }
}
