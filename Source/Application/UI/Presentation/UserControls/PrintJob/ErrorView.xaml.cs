using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public partial class ErrorView : UserControl
    {
        public ErrorView(ErrorViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
