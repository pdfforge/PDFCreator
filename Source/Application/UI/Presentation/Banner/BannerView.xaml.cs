using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.Banner
{
    public partial class BannerView : UserControl
    {
        private readonly IBannerManager _bannerManager;

        public BannerView(BannerViewModel bannerViewModel, IBannerManager bannerManager)
        {
            _bannerManager = bannerManager;
            DataContext = bannerViewModel;
            InitializeComponent();

            Loaded += async (sender, args) => await SetBanner();
        }

        private async Task SetBanner()
        {
            // No banner currently loaded
            if (BannerGrid.Children.Count == 0)
            {
                var bannerControl = await _bannerManager.GetBanner(BannerSlots.Home);
                if (bannerControl != null)
                {
                    BannerGrid.Children.Add(bannerControl);
                    FrequentTipsControl.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
