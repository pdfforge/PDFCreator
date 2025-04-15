using ICSharpCode.SharpZipLib;
using pdfforge.Banners;
using pdfforge.PDFCreator.Core.Services.Trial;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.Banner
{
    public interface IBannerManagerWrapper
    {
        Task<UIElement> GetBanner(string slot, IDictionary<string, string> optionalParameters = null);
    }

    public class BannerManagerWrapper : IBannerManagerWrapper
    {
        private readonly IBannerManager _bannerManager;
        private readonly ICampaignHelper _campaignHelper;
        private readonly EditionHelper _editionHelper;

        private bool IsBannerEnabled
        {
            get
            {
                if (_editionHelper.IsFreeEdition)
                    return true;

                if (_campaignHelper.IsTrial)
                    return true;

                return false;
            }
        }

        public BannerManagerWrapper(IBannerManager bannerManager, ICampaignHelper campaignHelper, EditionHelper editionHelper)
        {
            _bannerManager = bannerManager;
            _campaignHelper = campaignHelper;
            _editionHelper = editionHelper;

            // Add hard references to make sure they are not removed during build
            var _ = typeof(Pidgin.Parser);
            _ = typeof(SharpZipBaseException);
        }

        public async Task<UIElement> GetBanner(string slot, IDictionary<string, string> optionalParameters = null)
        {
            if (!IsBannerEnabled)
                return null;

            var banner = await _bannerManager.GetRandomBanner(slot, optionalParameters);
            if (banner is InlineBanner inlineBanner)
            {
                _campaignHelper.ExtendLicenseUrl = GetBannerUrlWithParameters(inlineBanner, optionalParameters);
                return inlineBanner.UiElement.Value;
            }

            return null;
        }

        private static string GetBannerUrlWithParameters(IBanner banner, IDictionary<string, string> optionalParameters)
        {
            var url = banner.BannerDefinition.Link;

            if (string.IsNullOrEmpty(url))
                return string.Empty;

            // Apply link optionalParameters first if they exist
            if (banner.BannerDefinition.LinkParameters != null)
                url = UrlHelper.AddUrlParameters(banner.BannerDefinition.Link, banner.BannerDefinition.LinkParameters);

            // Set the source parameter to the campaign defined in the banner
            if (!string.IsNullOrWhiteSpace(banner.BannerDefinition.Campaign))
                url = UrlHelper.AddUrlParameters(url, "source", banner.BannerDefinition.Campaign, true);

            // Overwrite duplicate banner params if optional params are provided
            if (optionalParameters != null)
                url = UrlHelper.AddUrlParameters(url, optionalParameters, true);

            return url;
        }
    }

    public class EmptyBannerManagerWrapper : IBannerManagerWrapper
    {
        public Task<UIElement> GetBanner(string slot, IDictionary<string, string> optionalParameters = null)
        {
            return Task.FromResult((UIElement)null);
        }
    }
}
