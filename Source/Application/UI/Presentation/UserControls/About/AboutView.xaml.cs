using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public partial class AboutView : UserControl
    {
        public AboutView(AboutViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
