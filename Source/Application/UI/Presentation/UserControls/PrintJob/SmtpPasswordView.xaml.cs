using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    /// <summary>
    /// Interaction logic for SmtpPasswordView.xaml
    /// </summary>
    public partial class SmtpPasswordView : UserControl
    {
        public SmtpPasswordView(SmtpJobStepPasswordViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
