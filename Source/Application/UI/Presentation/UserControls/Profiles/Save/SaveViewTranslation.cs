using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class SaveViewTranslation : ITranslatable
    {
        public string InteractiveToggle { get; private set; } = "Interactive";
        public string InteractiveHint { get; private set; } = "Shows a window to select the filename, output format etc.";
        public string AutosaveToggle { get; private set; } = "Automatic";
        public string AutosaveHint { get; private set; } = "Converts all documents without asking for settings.\nThis requires all necessary settings to be set in advance.";

        public string SaveTabText { get; private set; } = "Save";
        public string SaveTitle { get; private set; } = "Save";
        public string SelectFilename { get; private set; } = "Filename:";
        public string DirectoryLabel { get; private set; } = "Target directory:";
        public string LeaveDirectoryEmpty { get; private set; } = "Hint: Leave the target directory blank, to use the recently used target directory.";

        public string SkipPrintDialog { get; private set; } = "Skip job dialog (start with save dialog)";
        public string SkipSendFailures { get; private set; } = "Skip send actions that fail";
        public string WarnSendFailures { get; private set; } = "Warn when a send action is skipped";

        public string SaveOnlyTemporary { get; private set; } = "Only save file temporarily";
        public string NoSendActionEnabledHintInfo { get; private set; } = "Saving files temporarily without any enabled send action, will only store files in a temporary folder";

        public string ShowPreview { get; private set; } = "Show preview";

        public string TemporarySaveFilesHint { get; private set; } = "Select this to only save files in a temporary directory, e.g. if sent by email";
        public string SelectTargetDirectory { get; private set; } = "Select target directory";

        public string OverWriteFiles { get; private set; } = "Overwrite files";
        public string EnsureUniqueFilenames { get; private set; } = "Ensure unique filenames - don't overwrite existing files";
        public string AutoMerge { get; private set; } = "Auto merge - Append new files to the existing file (only for PDF)";
        public string AutoMergeBeforeModifyActions { get; private set; } = "Auto merge - Append new files to the existing file before the modify actions are applied (only for PDF)";

        public string UniqueFilenameHint { get; private set; } = "If a file already exists, an incrementing number is added to the filename, e.g. 'file_5.pdf'";

        public string ShowQuickActions { get; private set; } = "Show quick actions after the documents were converted";

        public string ExistingFileDescription { get; private set; } = "Behavior for existing file:";

        public string ShowAllNotifications { get; private set; } = "Show tray notification after converting a document";
        public string ShowOnlyErrorNotifications { get; private set; } = "Only show notification for errors";
        public string EnsureUserTokensNotSelected { get; private set; } = "Please be aware: By utilizing user tokens in the target directory users are able to save files to any directory the service has write permission.";
    }
}
