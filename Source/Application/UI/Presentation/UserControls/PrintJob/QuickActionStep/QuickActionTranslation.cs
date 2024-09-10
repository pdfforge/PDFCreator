using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep
{
    public class QuickActionTranslation : ITranslatable
    {
        public string OpenPDFArchitect { get; private set; } = "Open with PDF Architect";
        public string QuickActionWorkflowStepTitle { get; private set; } = "Quick Actions";
        public string OpenDefaultProgram { get; private set; } = "Open with default viewer";
        public string OpenExplorer { get; private set; } = "Open folder";
        public string SendEmail { get; private set; } = "Send by e-mail";
        public string PrintFileWithArchitect { get; private set; } = "Print with PDF Architect";
        public string SelectFilename { get; private set; } = "Filename:";
        public string Directory { get; private set; } = "Directory:";
        public string OkButton { get; private set; } = "Ok";
        public string Open { get; private set; } = "Open";
        public string Send { get; private set; } = "Send";
        public string AttachTo { get; private set; } = "Attach to:";
        public string TotalFileSize { get; private set; } = "Total file size:";
        public string DropBoxSharedLink { get; private set; } = "DropBox:";

        public string DontShowUntilNextUpdate { get; private set; } = "Don't show Quick Actions until the next update";
        public string CopyToClipboard { get; private set; } = "Copy the full path to clipboard";
        public string OneDrivePrivateLink { get; private set; } = "OneDrive private link";
        public string OneDriveShareLink { get; private set; } = "OneDrive share link";
    }
}
