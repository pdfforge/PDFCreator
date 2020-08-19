using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Background
{
    public partial class BackgroundUserControl : UserControl, IRegionMemberLifetime
    {
        public bool KeepAlive { get; } = true;
        public BackgroundUserControl(BackgroundUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
