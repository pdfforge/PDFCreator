using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Regions;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background
{
    public partial class BackgroundUserControl : UserControl, IRegionMemberLifetime, IActionUserControl
    {
        public IActionViewModel ViewModel { get; private set; }

        public bool KeepAlive { get; } = true;

        public BackgroundUserControl(BackgroundUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
