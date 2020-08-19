using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public partial class LicenseExpirationReminderControl : UserControl
    {
        public static readonly DependencyProperty ShowManageLicenseButtonDependencyProperty = DependencyProperty.Register(
            "ShowManageLicenseButton",
                typeof(bool),
                typeof(LicenseExpirationReminderControl), new PropertyMetadata(false));

        public LicenseExpirationReminderControl()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
                DataContext = RestrictedServiceLocator.Current.GetInstance<LicenseExpirationReminderViewModel>();

            InitializeComponent();
        }

        public bool ShowManageLicenseButton
        {
            get
            {
                var value = GetValue(ShowManageLicenseButtonDependencyProperty);
                return value != null && (bool)value;
            }
            set
            {
                SetValue(ShowManageLicenseButtonDependencyProperty, value);
            }
        }
    }
}
