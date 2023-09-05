using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.Utilities.Web;
using Prism.Events;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Banner
{
    public partial class BannerView : UserControl
    {
        private readonly IBannerManagerWrapper _bannerManagerWrapper;
        private readonly TrackingParameters _trackingParameters;

        private bool _isTrial = false;

        public BannerView(BannerViewModel bannerViewModel, IBannerManagerWrapper bannerManagerWrapper, TrackingParameters trackingParameters, IEventAggregator eventAggregator, IDispatcher dispatcher)
        {
            _bannerManagerWrapper = bannerManagerWrapper;
            _trackingParameters = trackingParameters;
            DataContext = bannerViewModel;
            ViewModel = bannerViewModel;
            InitializeComponent();

            var eventSubscription = eventAggregator.GetEvent<TrialStatusChangedEvent>().Subscribe(
                () => dispatcher.InvokeAsync(SetBanner));

            Loaded += async (sender, args) => await SetBanner();
            Unloaded += (sender, args) => eventSubscription?.Dispose();
        }

        private BannerViewModel ViewModel { get; }

        private async Task SetBanner()
        {
            var trialStatusChanged = _isTrial != ViewModel.CampaignHelper.IsTrial;

            if (trialStatusChanged)
            {
                _isTrial = ViewModel.CampaignHelper.IsTrial;
                BannerGrid.Children.Clear();
            }

            // No banner currently loaded
            if (BannerGrid.Children.Count == 0)
            {
                IDictionary<string, string> trackingParameters = null;
                if (ViewModel.CampaignHelper.IsTrial)
                    trackingParameters = _trackingParameters.ToParamList();
                var bannerControl = await _bannerManagerWrapper.GetBanner(BannerSlots.Home, trackingParameters);
                if (bannerControl != null)
                {
                    BannerGrid.Children.Add(bannerControl);
                    FrequentTipsControl.Visibility = Visibility.Collapsed;
                }
            }

            if (BannerGrid.Children.Count == 0)
                FrequentTipsControl.Visibility = ViewModel.FrequentBannerIsVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
