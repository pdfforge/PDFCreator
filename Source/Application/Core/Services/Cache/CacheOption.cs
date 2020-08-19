using System;

namespace pdfforge.PDFCreator.Core.Services.Cache
{
    public class CacheOption
    {
        public CacheOption(string cacheDirectory, TimeSpan maxCacheDuration)
        {
            CacheDirectory = cacheDirectory;
            MaxCacheDuration = maxCacheDuration;
        }

        public string CacheDirectory { get; }
        public TimeSpan MaxCacheDuration { get; }
    }
}
