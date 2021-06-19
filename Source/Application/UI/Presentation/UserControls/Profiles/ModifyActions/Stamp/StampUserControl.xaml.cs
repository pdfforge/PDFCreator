using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background;
using Prism.Regions;
using UserControl = System.Windows.Controls.UserControl;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Stamp
{
    public partial class StampUserControl : UserControl, IRegionMemberLifetime, IActionUserControl
    {
        public bool KeepAlive { get; } = true;

        public StampUserControl(StampUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }

        public IActionViewModel ViewModel { get; }
    }
}
