using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class DebugSettingsTranslation : ITranslatable
    {
        public string DebugTabTitle { get; private set; } = "Debug";
        public string ClearLogFileButtonContent { get; private set; } = "Clear log file";
        public string DefaultSettingsButtonContent { get; private set; } = "Restore";
        public string DefaultSettingsControlHeader { get; private set; } = "Default Settings";
        public string ExportSettingsControlHeader { get; private set; } = "Export Settings";
        public string LoadSettingsButtonContent { get; private set; } = "Load settings from file";
        public string LogFileClearedText { get; private set; } = "Log file cleared";
        public string LoggingControlHeader { get; private set; } = "Logging";
        public string LoggingLevelLabelText { get; private set; } = "Logging Level:";
        public string PdfCreatorTestpageButtonContent { get; private set; } = "Print PDFCreator Test Page";
        public string SaveSettingsButtonContent { get; private set; } = "Save settings to file";
        public string ShowLogFileButtonContent { get; private set; } = "Show log file";
        public string ShowEventLogButtonContent { get; private set; } = "Open Event Log";
        public string TestPagesControlHeader { get; private set; } = "Test Pages";
        public string AddAction { get; protected set; } = "Add Action";
        public string WindowsTestpageButtonContent { get; private set; } = "Print Windows Test Page";
        public string RestoreDefaultSettingsMessage { get; private set; } = "Do you really want to restore the default settings? The current settings will be lost.";
        public string NoLogFile { get; private set; } = "No log file";
        public string NoLogFileAvailable { get; private set; } = "There is currently no log file available.";
        public string RestoreDefaultSettingsTitle { get; private set; } = "Restore Default Settings";
        public string PrintTestPageTooltip { get; private set; } = "Print a test page with the current profile. You will be asked to save the settings if required.";
        
    }
}
