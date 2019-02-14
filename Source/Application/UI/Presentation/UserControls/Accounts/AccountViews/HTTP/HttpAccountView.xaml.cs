using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    /// <summary>
    /// Interaction logic for HttpAccountView.xaml
    /// </summary>
    public partial class HttpAccountView : UserControl
    {
        public HttpAccountView(HttpAccountViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
