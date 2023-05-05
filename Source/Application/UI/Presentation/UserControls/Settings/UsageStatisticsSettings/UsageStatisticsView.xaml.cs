using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings
{
   
    public partial class UsageStatisticsView : UserControl
    {
        public UsageStatisticsView(UsageStatisticsViewModelBase viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
