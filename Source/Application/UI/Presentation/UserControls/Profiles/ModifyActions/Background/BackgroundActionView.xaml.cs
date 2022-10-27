using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Regions;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background
{
    public partial class BackgroundActionView : UserControl, IRegionMemberLifetime, IActionView
    {
        public IActionViewModel ViewModel { get; private set; }

        public bool KeepAlive { get; } = true;

        public BackgroundActionView(BackgroundActionViewModel viewModel)
        {
            DataContext = viewModel;
            ViewModel = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
