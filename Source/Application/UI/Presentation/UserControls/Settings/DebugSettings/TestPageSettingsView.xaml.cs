using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    /// <summary>
    ///     Interaction logic for TestPageSettingsView.xaml
    /// </summary>
    public partial class TestPageSettingsView : UserControl
    {
        public TestPageSettingsView(TestPageSettingsViewModelBase viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
