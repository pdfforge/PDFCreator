using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using pdfforge.PDFCreator.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Banners.UnitTest
{
    [TestFixture]
    public class BannerDownloaderTest
    {
        private readonly string _appVersion = "1.2.3";
        private readonly string _appLanguage = "en";
        private readonly string _cacheFolder = "X:\\";
        private string _bannerDownloadUrl = "https://some-domain/banners";

        private IFileCache _fileCache;
        private IHashUtil _hashUtil;
        private IDownloader _downloader;
        private BannerData _bannerData;

        [SetUp]
        public void Setup()
        {
            _bannerDownloadUrl = "https://some-domain/banners";
            _fileCache = Substitute.For<IFileCache>();
            _hashUtil = Substitute.For<IHashUtil>();
            _downloader = Substitute.For<IDownloader>();

            _bannerData = new BannerData
            {
                Banners = new[]
                {
                    new BannerDefinition
                    {
                        BundleId = "bundle1",
                        Version = 1,
                        DownloadLink = "https://banner1"
                    },
                }
            };

            _downloader.DownloadStringTaskAsync(null).ReturnsForAnyArgs(x => JsonConvert.SerializeObject(_bannerData));

            _fileCache.GetCacheFilePath(Arg.Any<string>()).Returns(x => _cacheFolder + x.Arg<string>());
        }

        private BannerDownloader BuildBannerDownloader()
        {
            var bannerOptions = new BannerOptions(_bannerDownloadUrl, String.Empty, TimeSpan.MinValue);
            return new BannerDownloader(_fileCache, bannerOptions, _hashUtil, _downloader);
        }

        private void MakeIndexDownloadFail()
        {
            _downloader.DownloadStringTaskAsync(null).ReturnsForAnyArgs(async x => throw new WebException());
        }

        [Test]
        public async Task GetBanners_BuildsUrlCorrectly()
        {
            var bannerDownloader = BuildBannerDownloader();

            var banner = await bannerDownloader.GetAllBannersAsync(_appVersion, _appLanguage);

            await _downloader.Received(1).DownloadStringTaskAsync(_bannerDownloadUrl + $"?lang={_appLanguage}&version={_appVersion}");
        }

        [Test]
        public async Task GetBanners_BuildsUrlWithParamsCorrectly()
        {
            _bannerDownloadUrl += "?a=b";

            var bannerDownloader = BuildBannerDownloader();

            var banner = await bannerDownloader.GetAllBannersAsync(_appVersion, _appLanguage);

            await _downloader.Received(1).DownloadStringTaskAsync(_bannerDownloadUrl + $"&lang={_appLanguage}&version={_appVersion}");
        }

        [Test]
        public async Task GetBanners_OnException_ReturnsEmptyList()
        {
            MakeIndexDownloadFail();

            var bannerDownloader = BuildBannerDownloader();

            var banner = await bannerDownloader.GetAllBannersAsync(_appVersion, _appLanguage);

            Assert.IsEmpty(banner);
        }

        [Test]
        public async Task GetBanners_CacheEmpty_DownloadsData()
        {
            var bannerDownloader = BuildBannerDownloader();

            var banners = await bannerDownloader.GetAllBannersAsync(_appVersion, _appLanguage);
            var expectedBanner = _bannerData.Banners[0];

            await _downloader.Received(1).DownloadStringTaskAsync(_bannerDownloadUrl + $"?lang={_appLanguage}&version={_appVersion}");
            await _downloader.Received(1).OpenReadTaskAsync(expectedBanner.DownloadLink);

            Assert.AreEqual(_bannerData.Banners[0].BundleId, banners.First().BundleId);

            await _fileCache.Received().SaveFileAsync($"{expectedBanner.BundleId}-v{expectedBanner.Version}.zip", Arg.Any<Stream>());
        }

        [Test]
        public async Task GetBanners_FailsOnWriteBanner_DoesNotFail()
        {
            _fileCache
                .When(x => x.SaveFileAsync(Arg.Any<string>(), Arg.Any<Stream>()))
                .Do(x => throw new Exception());

            var bannerDownloader = BuildBannerDownloader();

            var banners = await bannerDownloader.GetAllBannersAsync(_appVersion, _appLanguage);
            var expectedBanner = _bannerData.Banners[0];

            await _downloader.Received(1).DownloadStringTaskAsync(_bannerDownloadUrl + $"?lang={_appLanguage}&version={_appVersion}");
            await _downloader.Received(1).OpenReadTaskAsync(expectedBanner.DownloadLink);

            Assert.IsEmpty(banners);
        }

        [Test]
        public async Task GetBanners_CacheExists_DoesNotDownloadData()
        {
            _bannerDownloadUrl += "?a=b";
            var bannerDownloader = BuildBannerDownloader();

            var indexFile = "index_en.json";
            _fileCache.FileExists(indexFile).Returns(true);
            _fileCache.ReadFile(indexFile).Returns(JsonConvert.SerializeObject(_bannerData));

            var banners = await bannerDownloader.GetAllBannersAsync(_appVersion, _appLanguage);

            await _downloader.DidNotReceive().DownloadStringTaskAsync(_bannerDownloadUrl + $"&lang={_appLanguage}&version={_appVersion}");
            await _downloader.Received(1).OpenReadTaskAsync(_bannerData.Banners[0].DownloadLink);

            Assert.AreEqual(_bannerData.Banners[0].BundleId, banners.First().BundleId);
        }

        [Test]
        public async Task GetBanners_CacheThrowsException_DownloadsData()
        {
            _bannerDownloadUrl += "?a=b";
            var bannerDownloader = BuildBannerDownloader();

            var indexFile = "index_en.json";
            _fileCache.FileExists(indexFile).Returns(true);
            _fileCache.ReadFile(indexFile).Returns(x => throw new Exception());

            var banners = await bannerDownloader.GetAllBannersAsync(_appVersion, _appLanguage);

            await _downloader.Received(1).DownloadStringTaskAsync(_bannerDownloadUrl + $"&lang={_appLanguage}&version={_appVersion}");
            await _downloader.Received(1).OpenReadTaskAsync(_bannerData.Banners[0].DownloadLink);

            Assert.AreEqual(_bannerData.Banners[0].BundleId, banners.First().BundleId);
        }

        [Test]
        public async Task GetBanners_StaleCacheExists_UsesStaleCacheAfterFailedDownload()
        {
            _bannerDownloadUrl += "?a=b";
            MakeIndexDownloadFail();

            var bannerDownloader = BuildBannerDownloader();

            var indexFile = "index_en.json";
            _fileCache.FileExists(indexFile, true).Returns(true);
            _fileCache.ReadFile(indexFile).Returns(JsonConvert.SerializeObject(_bannerData));

            var banners = await bannerDownloader.GetAllBannersAsync(_appVersion, _appLanguage);

            await _downloader.Received(1).DownloadStringTaskAsync(_bannerDownloadUrl + $"&lang={_appLanguage}&version={_appVersion}");
            await _downloader.Received(1).OpenReadTaskAsync(_bannerData.Banners[0].DownloadLink);

            Assert.AreEqual(_bannerData.Banners[0].BundleId, banners.First().BundleId);
        }

        [Test]
        public async Task GetBanners_DownloadDefinitions_UsesCacheCorrectly()
        {
            _bannerDownloadUrl += "?a=b";
            MakeIndexDownloadFail();

            var bannerDownloader = BuildBannerDownloader();

            await bannerDownloader.GetAllBannersAsync(_appVersion, _appLanguage);

            Received.InOrder(() =>
            {
                // Check cache with cache lifetime first (file does not exist)
                _fileCache.FileExists("index_en.json", false);
                // Then download file (throws error)
                _downloader.DownloadStringTaskAsync(_bannerDownloadUrl + $"&lang={_appLanguage}&version={_appVersion}");
                // Finally, use stale file from cache
                _fileCache.FileExists("index_en.json", true);
            });
        }

        [Test]
        public async Task GetSingleBanner_VerifiesHash()
        {
            var bannerDownloader = BuildBannerDownloader();

            var expectedBanner = _bannerData.Banners[0];
            var zipFilename = expectedBanner.BundleId + "-v" + expectedBanner.Version + ".zip";

            _fileCache.FileExists(zipFilename, true).Returns(true);
            _hashUtil.VerifyFileMd5(_cacheFolder + zipFilename, Arg.Any<string>()).Returns(true);

            var banners = await bannerDownloader.GetAllBannersAsync(_appVersion, _appLanguage);

            await _downloader.Received(1).DownloadStringTaskAsync(_bannerDownloadUrl + $"?lang={_appLanguage}&version={_appVersion}");
            await _fileCache.DidNotReceive().SaveFileAsync(zipFilename, Arg.Any<Stream>());

            Assert.AreEqual(_bannerData.Banners[0].BundleId, banners.First().BundleId);
        }
    }
}
