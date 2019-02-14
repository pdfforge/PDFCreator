using pdfforge.PDFCreator.UI.Presentation.Banner;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public partial class PrintJobView : UserControl
    {
        private readonly IBannerManager _bannerManager;

        public PrintJobView(PrintJobViewModel viewModel, IBannerManager bannerManager)
        {
            _bannerManager = bannerManager;
            DataContext = viewModel;
            InitializeComponent();

            Loaded += async (sender, args) => await SetBanner();
        }

        private async Task SetBanner()
        {
            var bannerControl = await _bannerManager.GetBanner(BannerSlots.PrintJob);
            if (bannerControl != null)
            {
                BannerRegion.Children.Add(bannerControl);
            }
        }
    }
}
