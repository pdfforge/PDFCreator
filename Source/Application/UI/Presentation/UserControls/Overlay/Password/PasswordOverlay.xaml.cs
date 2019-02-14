using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay.Password
{
    public partial class PasswordOverlay : UserControl
    {
        public PasswordOverlay(PasswordOverlayViewModel overlayViewModel)
        {
            DataContext = overlayViewModel;

            Loaded += (sender, e) =>
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
