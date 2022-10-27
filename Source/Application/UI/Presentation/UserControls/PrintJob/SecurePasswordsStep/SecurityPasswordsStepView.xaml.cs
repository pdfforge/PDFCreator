using System.Windows.Controls;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public partial class SecurityPasswordsStepView : UserControl
    {
        public SecurityPasswordsStepView(SecurityPasswordsStepViewModel viewModel)
        {
            DataContext = viewModel;

            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();
        }
    }
}
