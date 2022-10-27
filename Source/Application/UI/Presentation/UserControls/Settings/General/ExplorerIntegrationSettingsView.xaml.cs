using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    /// <summary>
    ///     Interaction logic for ExplorerIntegrationSettingsView.xaml
    /// </summary>
    public partial class ExplorerIntegrationSettingsView : UserControl
    {
        public ExplorerIntegrationSettingsView()
        {
            InitializeComponent();
        }
        
        public void SetDataContext(ExplorerIntegrationSettingsViewModel viewModel)
        {
            DataContext = viewModel;
        }
    }
}
