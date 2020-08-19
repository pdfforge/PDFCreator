using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    /// <summary>
    ///     Interaction logic for TestPageSettingsView.xaml
    /// </summary>
    public partial class WorkflowEditorTestPageUserControl : UserControl
    {
        public WorkflowEditorTestPageUserControl(WorkflowTestPageSettingsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
