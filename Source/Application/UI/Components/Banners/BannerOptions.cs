using pdfforge.PDFCreator.Core.Services.Cache;
using System;

namespace Banners
{
    public class BannerOptions : CacheOption
    {
        public BannerOptions(string indexUrl, string cacheDirectory, TimeSpan maxCacheDuration) : base(cacheDirectory, maxCacheDuration)
        {
            IndexUrl = indexUrl;
        }

        public string IndexUrl { get; }
    }
}
