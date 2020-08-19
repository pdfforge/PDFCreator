using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab
{
    /// <summary>
    /// Interaction logic for WatermarkView.xaml
    /// </summary>
    public partial class WatermarkView : UserControl
    {
        public bool KeepAlive { get; } = true;

        public WatermarkView(WatermarkViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
