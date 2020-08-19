using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Cover
{
    public partial class CoverUserControl : UserControl, IRegionMemberLifetime
    {
        public bool KeepAlive { get; } = true;
        public CoverUserControl(CoverUserControlViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
