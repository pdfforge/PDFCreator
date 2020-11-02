using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public partial class DebugSettingView : UserControl
    {
        public DebugSettingView(DebugSettingsViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            TransposerHelper.Register(this, vm);
        }
    }
}
