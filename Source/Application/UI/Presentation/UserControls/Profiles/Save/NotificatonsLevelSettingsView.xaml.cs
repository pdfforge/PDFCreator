using pdfforge.PDFCreator.Core.ServiceLocator;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public partial class NotificationsLevelSettingsView : UserControl
    {
        public NotificationsLevelSettingsView()
        {
            if (RestrictedServiceLocator.IsLocationProviderSet)
                DataContext = RestrictedServiceLocator.Current.GetInstance<NotificationsLevelSettingsViewModel>();

            InitializeComponent();
        }

        public NotificationsLevelSettingsView(NotificationsLevelSettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
