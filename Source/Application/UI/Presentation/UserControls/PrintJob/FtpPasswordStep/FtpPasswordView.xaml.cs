using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    /// <summary>
    /// Interaction logic for FtpPasswordView.xaml
    /// </summary>
    public partial class FtpPasswordView : UserControl
    {
        public FtpPasswordView(FtpJobStepPasswordViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
