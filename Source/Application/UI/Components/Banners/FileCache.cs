using System;
using System.IO;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace Banners
{
    public interface IFileCache
    {
        string CacheDirectory { get; }

        bool FileExists(string filename, bool ignoreAge = false);

        void SaveFile(string filename, string contents);

        Task SaveFileAsync(string filename, Stream contents);

        string ReadFile(string filename);

        void DeleteFile(string filename);

        string GetCacheFilePath(string filename);
    }

    public class FileCache : IFileCache
    {
        public string CacheDirectory { get; }

        private readonly TimeSpan _maxCacheDuration;

        public FileCache(BannerOptions options)
        {
            CacheDirectory = options.CacheDirectory;
            _maxCacheDuration = options.MaxCacheDuration;
        }

        public bool FileExists(string filename, bool ignoreAge = false)
        {
            var fullPath = GetCacheFilePath(filename);

            if (!File.Exists(fullPath))
                return false;

            if (_maxCacheDuration == TimeSpan.MaxValue || ignoreAge)
                return true;

            var fileAge = DateTime.Now - File.GetLastWriteTime(fullPath);
            return fileAge < _maxCacheDuration;
        }

        public string GetCacheFilePath(string filename)
        {
            return PathSafe.Combine(CacheDirectory, filename);
        }

        public void SaveFile(string filename, string contents)
        {
            var fullPath = GetCacheFilePath(filename);

            if (!Directory.Exists(CacheDirectory))
                Directory.CreateDirectory(CacheDirectory);

            File.WriteAllText(fullPath, contents);
        }

        public async Task SaveFileAsync(string filename, Stream contents)
        {
            var fullPath = GetCacheFilePath(filename);

            if (!Directory.Exists(CacheDirectory))
                Directory.CreateDirectory(CacheDirectory);

            using (var file = File.Create(fullPath))
            {
                await contents.CopyToAsync(file);
            }
        }

        public string ReadFile(string filename)
        {
            var fullPath = GetCacheFilePath(filename);

            if (!Directory.Exists(CacheDirectory))
                Directory.CreateDirectory(CacheDirectory);

            return File.ReadAllText(fullPath);
        }

        public void DeleteFile(string filename)
        {
            var fullPath = GetCacheFilePath(filename);
            if (FileExists(filename))
                File.Delete(fullPath);
        }
    }
}
