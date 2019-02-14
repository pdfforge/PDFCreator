using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public partial class SignaturePasswordView : UserControl
    {
        public SignaturePasswordView(IPasswordButtonViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }

    public class SignaturePasswordOverlayView : SignaturePasswordView
    {
        public SignaturePasswordOverlayView(SignaturePasswordOverlayViewModel viewModel) : base(viewModel)
        {
        }
    }

    public class SignaturePasswordStepView : SignaturePasswordView
    {
        public SignaturePasswordStepView(SignaturePasswordStepViewModel viewModel) : base(viewModel)
        {
        }
    }
}
