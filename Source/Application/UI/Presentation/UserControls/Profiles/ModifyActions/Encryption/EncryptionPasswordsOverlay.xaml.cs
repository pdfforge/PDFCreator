using System.Windows.Controls;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Encryption
{
    public partial class EncryptionPasswordsOverlay : UserControl
    {
        public EncryptionPasswordsOverlay(EncryptionPasswordsOverlayViewModel viewModel)
        {
            DataContext = viewModel;

            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();
        }
    }
}
