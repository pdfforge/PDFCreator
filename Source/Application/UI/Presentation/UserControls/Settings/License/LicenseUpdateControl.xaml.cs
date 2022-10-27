using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.License
{
    public partial class LicenseUpdateControl : UserControl
    {
        private readonly LicenseSettingsView _licenseSettingsView;

        public LicenseUpdateControl(LicenseSettingsView licenseSettingsView)
        {
            _licenseSettingsView = licenseSettingsView;
            InitializeComponent();
            _licenseSettingsView.Margin = new Thickness(10);
            PartContent.Children.Add(licenseSettingsView);
        }

        private Window FindWindow(UserControl userControl)
        {
            FrameworkElement control = userControl;
            while (control.Parent != null)
            {
                var parent = (FrameworkElement) control.Parent;

                if (parent is Window)
                    return (Window) parent;

                control = parent;
            }

            return null;
        }

        private void LicenseUpdateControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            var window = FindWindow(this);
            _licenseSettingsView.ViewModel.CloseLicenseWindowEvent += (s, ev) => window?.Close();
        }
    }
}
