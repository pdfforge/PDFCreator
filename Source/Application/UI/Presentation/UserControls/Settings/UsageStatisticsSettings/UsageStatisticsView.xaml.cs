using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.UsageStatisticsSettings
{
   
    public partial class UsageStatisticsView : UserControl
    {
        public UsageStatisticsView()
        {
            InitializeComponent();
        }

        public void SetDataContext(UsageStatisticsViewModelBase viewModel)
        {
            DataContext = viewModel;
        }
    }
}
