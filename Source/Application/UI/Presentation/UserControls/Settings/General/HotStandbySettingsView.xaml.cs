using System.Windows.Controls;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public partial class HotStandbySettingsView : UserControl
    {
        public HotStandbySettingsView(HotStandbySettingsViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
        }
    }
}
