using System.Windows.Controls;


namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    
    public partial class HomeViewSettingsView : UserControl
    {
        public HomeViewSettingsView()
        {
            InitializeComponent();
        }
        
        public void SetDataContext(HomeViewSettingsViewModel viewModel)
        {
            DataContext = viewModel;
        }
    }
}
