using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.Services.Trial;
using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using Prism.Mvvm;

namespace pdfforge.PDFCreator.UI.Presentation.Banner
{
    public class BannerViewModel : BindableBase
    {
        public ICampaignHelper CampaignHelper { get; }

        private readonly ICurrentSettings<ApplicationSettings> _applicationSettings;
        private readonly IGpoSettings _gpoSettings;

        public bool FrequentBannerIsVisible => _applicationSettings.Settings.EnableTips && !_gpoSettings.DisableTips;
        public FrequentTipsControlViewModel FrequentTipsControlViewModel { get; private set; }

        public BannerViewModel(FrequentTipsControlViewModel frequentTipsControlViewModel, ICurrentSettings<ApplicationSettings> applicationSettings, IGpoSettings gpoSettings, ICampaignHelper campaignHelper)
        {
            FrequentTipsControlViewModel = frequentTipsControlViewModel;
            CampaignHelper = campaignHelper;
            _applicationSettings = applicationSettings;
            _gpoSettings = gpoSettings;
            applicationSettings.SettingsChanged += (sender, args) =>
            {
                RaisePropertyChanged(nameof(FrequentBannerIsVisible));
            };
        }
    }

    public class DesignTimeBannerViewModel : BannerViewModel
    {
        public DesignTimeBannerViewModel() : base(new DesignTimeFrequentTipsControlViewModel(), new DesignTimeCurrentSettings<ApplicationSettings>(), null, new CampaignHelper())
        {
            //FrequentBannerIsVisible = true;
        }
    }
}
