using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public class LoadSettingsTranslation : ITranslatable
    {
        public string IniFileFilter { get; private set; } = "INI files (*.ini)|*.ini";
        public string InvalidSettings { get; private set; } = "Invalid settings";
        public string InvalidSettingsWarning { get; private set; } = "The file does not appear to contain valid PDFCreator settings.";
        public string LoadSettingsFromFileWarning { get; private set; } = "By loading the file all settings and profiles will be overwritten. Do you want to continue?";
        public string OverwriteAllSettings { get; private set; } = "Overwrite all settings";

        public string MissingPrinters { get; private set; } = "Missing Printers";
        public string UnusedPrinters { get; private set; } = "Unused Printers";
        public string AskAddMissingPrinters { get; private set; } = "The settings contain printers, that are currently not installed. Do you want to add these printers now?";
        public string AskDeleteUnusedPrinters { get; private set; } = "The settings contain printers, that are currently installed but not used. Do you want to delete these printers now?";

        private string SettingsFileName { get; set; } = "{0} Settings";
        public string ReplacedPasswords { get; set; } = "replaced passwords";

        public string FormatSettingsFileName(string productName) => string.Format(SettingsFileName, productName);
    }
}
