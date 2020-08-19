using System;

namespace pdfforge.PDFCreator.Core.Services.Cache
{
    public class FileCacheFactory : IFileCacheFactory
    {
        public FileCache GetFileCache(string cacheDirectory, TimeSpan maxDuration)
        {
            return new FileCache(new CacheOption(cacheDirectory, maxDuration));
        }
    }

    public interface IFileCacheFactory
    {
        FileCache GetFileCache(string cacheDirectory, TimeSpan maxDuration);
    }
}
