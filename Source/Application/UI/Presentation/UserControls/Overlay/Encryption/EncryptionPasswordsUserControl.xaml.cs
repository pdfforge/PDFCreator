using System.Windows.Controls;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Encryption
{
    public partial class EncryptionPasswordsUserControl : UserControl
    {
        public EncryptionPasswordsUserControl(EncryptionPasswordUserControlViewModel viewModel)
        {
            DataContext = viewModel;

            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();
        }
    }
}
