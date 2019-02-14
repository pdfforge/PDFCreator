using System;

namespace Banners
{
    public class BannerOptions
    {
        public BannerOptions(string indexUrl, string cacheDirectory, TimeSpan maxCacheDuration)
        {
            IndexUrl = indexUrl;
            CacheDirectory = cacheDirectory;
            MaxCacheDuration = maxCacheDuration;
        }

        public string IndexUrl { get; }
        public string CacheDirectory { get; }
        public TimeSpan MaxCacheDuration { get; }
    }
}
