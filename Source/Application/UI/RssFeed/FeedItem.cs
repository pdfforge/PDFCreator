using System;

namespace pdfforge.PDFCreator.UI.RssFeed
{
    [Serializable]
    public class FeedItem
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
    }
}
