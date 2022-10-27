using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    /// <summary>
    /// Interaction logic for SmtpPasswordStepView.xaml
    /// </summary>
    public partial class SmtpPasswordStepView : UserControl
    {
        public SmtpPasswordStepView(SmtpJobStepPasswordViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
