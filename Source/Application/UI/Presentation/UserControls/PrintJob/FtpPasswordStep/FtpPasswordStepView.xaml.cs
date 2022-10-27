using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    /// <summary>
    /// Interaction logic for FtpPasswordView.xaml
    /// </summary>
    public partial class FtpPasswordStepView : UserControl
    {
        public FtpPasswordStepView(FtpPasswordStepViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
