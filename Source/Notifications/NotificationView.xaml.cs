using System.Windows;
using ToastNotifications.Core;

namespace pdfforge.PDFCreator.UI.Notifications
{
    public partial class NotificationView : NotificationDisplayPart
    {
        public NotificationView(NotificationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            Visibility = Visibility.Hidden;
            parentWindow?.Close();
        }
    }
}
