using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public partial class SignaturePasswordStepView : UserControl
    {
        public SignaturePasswordStepView(SignaturePasswordStepViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}