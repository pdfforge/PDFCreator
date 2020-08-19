using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs
{
    public class SaveTabTranslation : ITranslatable
    {
        public string InteractiveToggle { get; private set; } = "Interactive";
        public string InteractiveHint { get; private set; } = "Shows a window to select the filename, output format etc.";
        public string AutosaveToggle { get; private set; } = "Automatic";
        public string AutosaveHint { get; private set; } = "Converts all documents without asking for settings. This requires more settings to be defined in the profile.";

        public string SaveTabText { get; private set; } = "Save";
        public string SaveTitle { get; private set; } = "Save";
        public string SelectFilename { get; private set; } = "Filename:";
        public string DirectoryLabel { get; private set; } = "Target Directory:";

        public string SkipPrintDialog { get; private set; } = "Skip print dialog (start with save dialog)";
        public string OpenFileAfterSaving { get; private set; } = "Open file after saving";
        public string SaveOnlyTemporary { get; private set; } = "Only save file temporarily";
        public string NoSendActionEnabledHintInfo { get; private set; } = "Please note that without enabling any send action, files will not be forwarded and only stored in a temporary folder";
        public string TemporarySaveFilesHint { get; private set; } = "Select this to only save files in a temporary directory, e.g. if sent by e-mail/SMTP";
        public string OpenWithPdfArchitect { get; private set; } = "Use PDF Architect for PDF files";
        public string ChooseFolder { get; private set; } = "Choose a folder";

        public string DontOverwriteFiles { get; private set; } = "Don't overwrite existing files";
        public string UniqueFilenameHint { get; private set; } = "If a file already exists, an incrementing number is added to the filename, i.e. 'file_5.pdf'";

        public string ShowQuickActions { get; private set; } = "Show quick actions after the documents were converted";
    }
}
