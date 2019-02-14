using pdfforge.PDFCreator.UI.Presentation.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Views.Windows
{
    public partial class UpdateDownloadWindow : UserControl
    {
        public UpdateDownloadWindow(UpdateDownloadWindowViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
