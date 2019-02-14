using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    /// <summary>
    /// Interaction logic for TimeServerAccountView.xaml
    /// </summary>
    public partial class TimeServerAccountView : UserControl
    {
        public TimeServerAccountView(TimeServerAccountViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
