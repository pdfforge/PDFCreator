using ICSharpCode.SharpZipLib.Zip;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.Utilities.Process;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using SystemInterface.IO;

namespace Banners
{
    public class BannerLoader
    {
        private readonly IDirectory _directory;
        private readonly IFileCache _fileCache;
        private readonly IProcessStarter _processStarter;
        private readonly IUsageStatisticsSender _usageStatisticsSender;
        private readonly IBannerMetricFactory _bannerMetricFactory;

        public BannerLoader(IDirectory directory, IFileCache fileCache, IProcessStarter processStarter, IUsageStatisticsSender usageStatisticsSender, IBannerMetricFactory bannerMetricFactory)
        {
            _directory = directory;
            _fileCache = fileCache;
            _processStarter = processStarter;
            _usageStatisticsSender = usageStatisticsSender;
            _bannerMetricFactory = bannerMetricFactory;
        }

        private bool DirectoryContainsBanner(string directory, BannerDefinition banner)
        {
            if (!_directory.Exists(directory))
                return false;

            var containsXaml = _directory.EnumerateFiles(directory, "*.xaml", SearchOption.AllDirectories).Any();
            var containsImages = _directory
                .EnumerateFiles(directory, "*", SearchOption.AllDirectories)
                .Any(f => f.EndsWith(".jpg", true, CultureInfo.InvariantCulture) || f.EndsWith(".png", true, CultureInfo.InvariantCulture));

            switch (banner.BundleType)
            {
                case BundleType.Xaml: return containsXaml;
                case BundleType.Image: return containsImages;
                default: throw new Exception("Unknown banner type!");
            }
        }

        public UIElement LoadBanner(BannerDefinition banner)
        {
            var baseName = PathSafe.Combine(_fileCache.CacheDirectory, $"{banner.BundleId}-v{banner.Version}");
            var filename = baseName + ".zip";
            var directory = baseName + "-extracted";

            if (!DirectoryContainsBanner(directory, banner))
            {
                ExtractZipFile(filename, directory);
            }

            FrameworkElement bannerControl;
            switch (banner.BundleType)
            {
                case BundleType.Xaml:
                    bannerControl = LoadXamlBanner(banner, directory);
                    break;

                case BundleType.Image:
                    bannerControl = LoadImageBanner(banner, directory);
                    break;

                default: throw new Exception("Unknown banner type!");
            }

            WpfHelper.RegisterOpen(bannerControl, url => HandleClick(banner, url));
            var loadOnce = new LoadOnceHandler();
            bannerControl.Loaded += (sender, args) => HandleLoaded(banner, loadOnce);

            return bannerControl;
        }

        private void HandleClick(BannerDefinition banner, string url)
        {
            url = UrlHelper.AddUrlParameters(url, banner.LinkParameters);

            if (!string.IsNullOrWhiteSpace(banner.Campaign))
                url = UrlHelper.AddUrlParameters(url, "cmp", banner.Campaign);

            try
            {
                _processStarter.Start(url);

                var metric = _bannerMetricFactory.BuildMetric(banner, BannerMetricType.Click);
                _usageStatisticsSender.SendAsync(metric);
            }
            catch { }
        }

        private void HandleLoaded(BannerDefinition banner, LoadOnceHandler loadOnce)
        {
            if (loadOnce.IsLoaded)
                return;

            try
            {
                loadOnce.IsLoaded = true;
                var metric = _bannerMetricFactory.BuildMetric(banner, BannerMetricType.Impression);
                _usageStatisticsSender.SendAsync(metric);
            }
            catch { }
        }

        private FrameworkElement LoadXamlBanner(BannerDefinition banner, string directory)
        {
            var xamlFile = _directory.EnumerateFiles(directory, "*.xaml", SearchOption.AllDirectories).First();

            using (var s = new FileStream(xamlFile, FileMode.Open))
            {
                return (FrameworkElement)XamlReader.Load(s);
            }
        }

        private FrameworkElement LoadImageBanner(BannerDefinition banner, string directory)
        {
            var image = _directory
                .EnumerateFiles(directory, "*", SearchOption.AllDirectories)
                .First(f => f.EndsWith(".jpg", true, CultureInfo.InvariantCulture) || f.EndsWith(".png", true, CultureInfo.InvariantCulture));

            var bannerControl = new ImageBanner();
            bannerControl.SetData(image, banner.Link);

            return bannerControl;
        }

        public void ExtractZipFile(string zipPath, string targetDir)
        {
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(zipPath, targetDir, null);
        }
    }

    internal class LoadOnceHandler
    {
        public bool IsLoaded { get; set; }
    }
}
