using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using Prism.Mvvm;

namespace pdfforge.PDFCreator.UI.Presentation.Banner
{
    public class BannerViewModel : BindableBase
    {
        private readonly ICurrentSettings<ApplicationSettings> _applicationSettings;
        private readonly IGpoSettings _gpoSettings;

        public bool FrequentBannerIsVisible => _applicationSettings.Settings.EnableTips && !_gpoSettings.DisableTips;
        public FrequentTipsControlViewModel FrequentTipsControlViewModel { get; private set; }

        public BannerViewModel(FrequentTipsControlViewModel frequentTipsControlViewModel, ICurrentSettings<ApplicationSettings> applicationSettings, IGpoSettings gpoSettings)
        {
            FrequentTipsControlViewModel = frequentTipsControlViewModel;
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
        public DesignTimeBannerViewModel() : base(new DesignTimeFrequentTipsControlViewModel(), new DesignTimeCurrentSettings<ApplicationSettings>(), null)
        {
            //FrequentBannerIsVisible = true;
        }
    }
}
