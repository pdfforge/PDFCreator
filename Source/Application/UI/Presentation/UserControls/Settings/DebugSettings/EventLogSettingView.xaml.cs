using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    /// <summary>
    ///     Interaction logic for LoggingSettingView.xaml
    /// </summary>
    public partial class EventLogSettingView : UserControl
    {
        public EventLogSettingView(EventLogSettingsViewModel viewmodel)
        {
            InitializeComponent();
            DataContext = viewmodel;
        }
    }
}
