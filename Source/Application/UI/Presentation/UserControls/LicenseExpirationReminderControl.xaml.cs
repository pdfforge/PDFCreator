using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public partial class LicenseExpirationReminderControl : UserControl
    {
        public LicenseExpirationReminderControl()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
                DataContext = RestrictedServiceLocator.Current.GetInstance<LicenseExpirationReminderViewModel>();

            InitializeComponent();
        }
    }
}
