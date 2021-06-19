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
        public string PdfCreatorTestpageButtonContent { get; private set; } = "Convert PDFCreator test page";
        public string SaveSettingsButtonContent { get; private set; } = "Save settings to file";
        public string SaveSettingsWithoutPasswordsButtonContent { get; private set; } = "Save settings without passwords";
        public string SaveSettingsWithoutPasswordsHint { get; private set; } = "Hint: We recommend to save your settings without passwords when sharing settings with our support";
        public string ShowLogFileButtonContent { get; private set; } = "Show log file";
        public string ShowEventLogButtonContent { get; private set; } = "Open event log";
        public string TestPagesControlHeader { get; private set; } = "Test Pages";
        public string AddAction { get; protected set; } = "Add action";
        public string WindowsTestpageButtonContent { get; private set; } = "Print Windows test page";
        public string RestoreDefaultSettingsMessage { get; private set; } = "Do you really want to restore the default settings? The current settings will be lost.";
        public string NoLogFile { get; private set; } = "No log file";
        public string NoLogFileAvailable { get; private set; } = "There is currently no log file available.";
        public string RestoreDefaultSettingsTitle { get; private set; } = "Restore Default Settings";
        public string PdfCreatorTestpage { get; private set; } = "PDFCreator Test Page";
        public string PrintTestPageTooltip { get; private set; } = "Convert a test page with the current profile. You will be asked to save the settings if required.";
    }
}
