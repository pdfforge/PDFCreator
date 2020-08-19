using Newtonsoft.Json;
using NLog;
using Optional;
using pdfforge.PDFCreator.Core.Services.Cache;
using pdfforge.PDFCreator.Core.Services.Download;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Banners
{
    public class BannerDownloader
    {
        private readonly BannerOptions _options;
        private readonly IHashUtil _hashUtil;
        private readonly IDownloader _downloader;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IFileCache _fileCache;

        public BannerDownloader(IFileCache fileCache, BannerOptions options, IHashUtil hashUtil, IDownloader downloader)
        {
            _fileCache = fileCache;
            _options = options;
            _hashUtil = hashUtil;
            _downloader = downloader;
        }

        private string GetIndexFilename(string languageIso2)
        {
            return "index_" + languageIso2 + ".json";
        }

        public async Task<IEnumerable<BannerDefinition>> GetAllBannersAsync(string applicationVersion, string languageIso2)
        {
            var banners = await GetBannerDefinitions(applicationVersion, languageIso2);

            var successfulBanners = new List<BannerDefinition>();

            foreach (var banner in banners)
            {
                var success = await FetchBanner(banner);
                if (success)
                    successfulBanners.Add(banner);
            }

            return successfulBanners;
        }

        private bool IsMd5Valid(BannerDefinition banner, string filename)
        {
            var fullPath = _fileCache.GetCacheFilePath(filename);
            return _hashUtil.VerifyFileMd5(fullPath, banner.DownloadMd5);
        }

        private async Task<bool> FetchBanner(BannerDefinition banner)
        {
            var cacheFilename = $"{banner.BundleId}-v{banner.Version}.zip";

            if (_fileCache.FileAvailable(cacheFilename, true) && IsMd5Valid(banner, cacheFilename))
                return true;

            try
            {
                using (var stream = await _downloader.OpenReadTaskAsync(banner.DownloadLink))
                {
                    await _fileCache.SaveFileAsync(cacheFilename, stream);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _fileCache.DeleteFile(cacheFilename);
                _logger.Debug(ex, "Could not fetch banners");
                return false;
            }
        }

        private async Task<IList<BannerDefinition>> GetBannerDefinitions(string applicationVersion, string languageIso2)
        {
            var definitions = GetCachedBannerDefinition(languageIso2, false);

            // if we don't have recent values in the cache, we try to download the banners
            if (!definitions.HasValue)
                definitions = await DownloadBannerDefinitions(_options.IndexUrl, applicationVersion, languageIso2);

            // if we can't download the banners, we accept an outdated cache
            if (!definitions.HasValue)
                definitions = GetCachedBannerDefinition(languageIso2, true);

            return definitions.ValueOr(new List<BannerDefinition>());
        }

        private Option<IList<BannerDefinition>> GetCachedBannerDefinition(string languageIso2, bool ignoreCacheAge)
        {
            var indexFilename = GetIndexFilename(languageIso2);
            if (_fileCache.FileAvailable(indexFilename, ignoreCacheAge))
            {
                try
                {
                    var json = _fileCache.ReadFile(indexFilename);
                    return ParseDefinition(json).Banners.Some();
                }
                catch
                {
                    // do nothing here and download file instead
                }
            }

            return Option.None<IList<BannerDefinition>>();
        }

        private async Task<Option<IList<BannerDefinition>>> DownloadBannerDefinitions(string baseUrl, string applicationVersion, string languageIso2)
        {
            try
            {
                var indexFilename = GetIndexFilename(languageIso2 ?? "en");
                var parameters = new Dictionary<string, string>()
                {
                    {"lang", languageIso2},
                    { "version", applicationVersion}
                };

                var fullUrl = UrlHelper.AddUrlParameters(baseUrl, parameters);

                var json = await _downloader.DownloadStringTaskAsync(fullUrl);

                var bannerData = ParseDefinition(json);

                _fileCache.SaveFile(indexFilename, json);

                return bannerData.Banners.Some();
            }
            catch (Exception ex)
            {
                _logger.Debug(ex, "Could not fetch banners");
                return Option.None<IList<BannerDefinition>>();
            }
        }

        private BannerData ParseDefinition(string json)
        {
            return JsonConvert.DeserializeObject<BannerData>(json);
        }
    }
}
