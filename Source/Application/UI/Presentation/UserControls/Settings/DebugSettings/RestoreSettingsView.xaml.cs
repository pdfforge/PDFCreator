using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    /// <summary>
    ///     Interaction logic for RestoreSettingsView.xaml
    /// </summary>
    public partial class RestoreSettingsView : UserControl
    {
        public RestoreSettingsView()
        {
            InitializeComponent();
        }

        public void SetDataContext(RestoreSettingsViewModelBase viewModel)
        {
            DataContext = viewModel;
        }
    }
}
