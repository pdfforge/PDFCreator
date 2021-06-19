using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions
{
    /// <summary>
    /// Interaction logic for WatermarkView.xaml
    /// </summary>
    public partial class WatermarkView : UserControl, IRegionMemberLifetime, IActionUserControl
    {
        public bool KeepAlive { get; } = true;

        public WatermarkView(WatermarkViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }

        public IActionViewModel ViewModel { get; }
    }
}
