using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Banner;
using pdfforge.PDFCreator.UI.Presentation.Events;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.Utilities.Web;
using Prism.Events;
using Prism.Regions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public partial class PrintJobView : UserControl
    {
        private readonly IBannerManagerWrapper _bannerManagerWrapper;
        private readonly IRegionManager _regionManager;
        private readonly TrackingParameters _trackingParameters;

        private PrintJobViewModel ViewModel { get; }

        public PrintJobView(PrintJobViewModel viewModel, IBannerManagerWrapper bannerManagerWrapper, IRegionManager regionManager, TrackingParameters trackingParameters, IEventAggregator eventAggregator, IDispatcher dispatcher)
        {
            _bannerManagerWrapper = bannerManagerWrapper;
            _regionManager = regionManager;
            _trackingParameters = trackingParameters;
            DataContext = viewModel;
            ViewModel = viewModel;
            InitializeComponent();

            TransposerHelper.Register(this, viewModel);

            // Using button.Focus() - see also https://stackoverflow.com/a/45139201
            SaveButton.Click += delegate { SaveButton.Focus(); };

            var eventSubscription = eventAggregator.GetEvent<TrialStatusChangedEvent>().Subscribe(() => dispatcher.InvokeAsync(UpdateBanner));

            Loaded += async (sender, args) => await SetBanner();
            Unloaded += (sender, args) => eventSubscription?.Dispose();
        }

        private async Task UpdateBanner()
        {
            _regionManager.Regions[PrintJobRegionNames.PrintJobViewBannerRegion].RemoveAll();
            await SetBanner();
        }

        private async Task SetBanner()
        {
            IDictionary<string, string> trackingParameters = null;
            if (ViewModel.CampaignHelper.IsTrial)
                trackingParameters = _trackingParameters.ToParamList();

            var bannerControl = await _bannerManagerWrapper.GetBanner(BannerSlots.PrintJob, trackingParameters);

            if (bannerControl != null)
            {
                _regionManager.AddToRegion(PrintJobRegionNames.PrintJobViewBannerRegion, bannerControl);
                _regionManager.Regions[PrintJobRegionNames.PrintJobViewBannerRegion].Activate(bannerControl);
            }

            ViewModel.HasBanner = bannerControl != null;
        }

        private void PrintJobView_OnLoaded(object sender, RoutedEventArgs e)
        {
            AccessKeyHelper.SetAccessKeys(sender);
        }
    }
}
