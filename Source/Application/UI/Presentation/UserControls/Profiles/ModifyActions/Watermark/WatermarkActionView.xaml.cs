using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background;
using Prism.Regions;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions
{
    /// <summary>
    /// Interaction logic for WatermarkActionView.xaml
    /// </summary>
    public partial class WatermarkActionView : UserControl, IRegionMemberLifetime, IActionView
    {
        public bool KeepAlive { get; } = true;

        public WatermarkActionView(WatermarkActionViewModel actionViewModel)
        {
            DataContext = actionViewModel;
            ViewModel = actionViewModel;
            TransposerHelper.Register(this, actionViewModel);
            InitializeComponent();
        }

        public IActionViewModel ViewModel { get; }
    }
}
