using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    /// <summary>
    ///     Interaction logic for LoggingSettingView.xaml
    /// </summary>
    public partial class LoggingSettingView : UserControl
    {
        public LoggingSettingView(LoggingSettingViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
        
    }
}
