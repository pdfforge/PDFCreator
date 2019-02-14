using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Printer
{
    public partial class PrinterView : UserControl
    {
        public PrinterView(PrinterViewModel viewModel)
        {
            DataContext = viewModel;
            TransposerHelper.Register(this, viewModel);
            InitializeComponent();
        }
    }
}
