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
        public string SelectFolder { get; private set; } = "Folder:";
        public string OkButton { get; private set; } = "OK";
        public string OpenWith { get; private set; } = "Open with";
        public string Send { get; private set; } = "Send";
        public string TotalFileSize { get; private set; } = "Total file size:";

        public string DontShowUntilNextUpdate { get; private set; } = "Don't show quick Actions until the next update";
    }
}
