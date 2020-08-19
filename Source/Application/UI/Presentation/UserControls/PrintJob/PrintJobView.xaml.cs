using pdfforge.PDFCreator.UI.Presentation.Banner;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Regions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public partial class PrintJobView : UserControl
    {
        private readonly IBannerManager _bannerManager;
        private readonly IRegionManager _regionManager;

        public PrintJobViewModel ViewModel { get; private set; }

        public PrintJobView(PrintJobViewModel viewModel, IBannerManager bannerManager, IRegionManager regionManager)
        {
            _bannerManager = bannerManager;
            _regionManager = regionManager;
            DataContext = viewModel;
            ViewModel = viewModel;
            InitializeComponent();

            TransposerHelper.Register(this, viewModel);

            // Using button.Focus() - see also https://stackoverflow.com/a/45139201
            SaveButton.Click += delegate { SaveButton.Focus(); };

            Loaded += async (sender, args) => await SetBanner();
        }

        private async Task SetBanner()
        {
            var bannerControl = await _bannerManager.GetBanner(BannerSlots.PrintJob);
            if (bannerControl != null)
            {
                _regionManager.AddToRegion(PrintJobRegionNames.PrintJobViewBannerRegion, bannerControl);
            }
        }
    }
}
