using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay
{
    /// <summary>
    /// Interaction logic for OverwriteOrAppendOverlay.xaml
    /// </summary>
    public partial class OverwriteOrAppendOverlay : UserControl
    {
        public OverwriteOrAppendOverlay(OverwriteOrAppendViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
