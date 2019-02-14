using Optional;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.Core.UsageStatistics;
using pdfforge.PDFCreator.UI.Presentation.Banner;
using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Process;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SystemInterface.IO;

namespace Banners
{
    public class OnlineBannerManager : IBannerManager
    {
        private readonly IVersionHelper _versionHelper;
        private readonly ILanguageProvider _languageProvider;

        private IList<BannerDefinition> _banners;
        private readonly BannerLoader _bannerLoader;
        private readonly BannerDownloader _bannerDownloader;

        public OnlineBannerManager(BannerOptions options, IDirectory directory, IProcessStarter processStarter, IVersionHelper versionHelper, ILanguageProvider languageProvider, IHashUtil hashUtil, IUsageStatisticsSender usageStatisticsSender, IBannerMetricFactory bannerMetricFactory)
        {
            var cache = new FileCache(options);

            _versionHelper = versionHelper;
            _languageProvider = languageProvider;
            _bannerLoader = new BannerLoader(directory, cache, processStarter, usageStatisticsSender, bannerMetricFactory);
            _bannerDownloader = new BannerDownloader(cache, options, hashUtil, new WebClientDownloader());
        }

        private async Task LoadBanners()
        {
            try
            {
                var version = _versionHelper.FormatWithThreeDigits();
                var language = _languageProvider.CurrentLanguage.Iso2;

                _banners = (await _bannerDownloader.GetAllBannersAsync(version, language)).ToList();
            }
            catch
            {
                _banners = new List<BannerDefinition>();
            }
        }

        public async Task<UIElement> GetBanner(string slot)
        {
            if (_banners == null)
                await LoadBanners();

            var selector = new BannerSelector();
            var selectedBanner = selector.SelectBanner(_banners, slot);

            var bannerControl = selectedBanner
                .FlatMap(banner => _bannerLoader.LoadBanner(banner).Some())
                .ValueOr((UIElement)null);

            return bannerControl;
        }
    }
}
