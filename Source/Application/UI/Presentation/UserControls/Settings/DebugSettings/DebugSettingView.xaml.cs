using System.Windows.Controls;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public partial class DebugSettingView : UserControl
    {
        public DebugSettingView(LoggingSettingViewModel loggingSettingViewModel, TestPageSettingsViewModelBase creatorTestPageSettingsViewModel, RestoreSettingsViewModelBase restoreSettingsViewModel, ExportSettingsViewModel exportSettingsViewModel, DebugSettingsViewModel vm)
        {
            DataContext = vm;
            InitializeComponent();
            TransposerHelper.Register(this, vm);
            LoggingSettingView.SetDataContext(loggingSettingViewModel);
            TestPageSettingsView.SetDataContext(creatorTestPageSettingsViewModel);
            RestoreSettingsView.SetDataContext(restoreSettingsViewModel);
            ExportSettingView.SetDataContext(exportSettingsViewModel);
        }
    }
}
