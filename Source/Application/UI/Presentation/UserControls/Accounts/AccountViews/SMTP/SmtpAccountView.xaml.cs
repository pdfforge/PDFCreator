using System.Windows;
using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    /// <summary>
    /// Interaction logic for SmtpAccountView.xaml
    /// </summary>
    public partial class SmtpAccountView : UserControl
    {
        private readonly SmtpAccountViewModel _viewModel;

        public SmtpAccountView(SmtpAccountViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = _viewModel;
            InitializeComponent();
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.SetAdressForEmptyUsername();
            UsernameTextbox.CaretIndex = _viewModel.Username.Length;
        }
    }
}
