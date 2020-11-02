using pdfforge.Banners;
using System.Threading.Tasks;
using System.Windows;
using IBannerManager = pdfforge.PDFCreator.UI.Presentation.Banner.IBannerManager;

namespace pdfforge.PDFCreator.Editions.PDFCreator.Wrapper
{
    internal class BannerManagerWrapper : IBannerManager
    {
        private readonly Banners.IBannerManager _bannerManager;

        public BannerManagerWrapper(Banners.IBannerManager bannerManager)
        {
            _bannerManager = bannerManager;
        }

        public async Task<UIElement> GetBanner(string slot)
        {
            var banner = await _bannerManager.GetRandomBanner(slot);

            if (banner is InlineBanner inlineBanner)
                return inlineBanner.UiElement.Value;

            return null;
        }
    }
}
