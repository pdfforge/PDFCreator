using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    /// <summary>
    ///     Interaction logic for ExportSettingView.xaml
    /// </summary>
    public partial class ExportSettingView : UserControl
    {
        public ExportSettingView(ExportSettingsViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
