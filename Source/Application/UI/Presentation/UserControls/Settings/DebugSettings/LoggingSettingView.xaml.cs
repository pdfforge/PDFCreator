using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    /// <summary>
    ///     Interaction logic for LoggingSettingView.xaml
    /// </summary>
    public partial class LoggingSettingView : UserControl
    {
        public LoggingSettingView()
        {
            InitializeComponent();
        }

        public void SetDataContext(LoggingSettingViewModel viewModel)
        {
            DataContext = viewModel;
        }
    }
}
