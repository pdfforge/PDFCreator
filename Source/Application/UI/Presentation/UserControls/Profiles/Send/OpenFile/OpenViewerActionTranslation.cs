using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.OpenFile
{
    public class OpenViewerActionTranslation : ITranslatable
    {
        public string OpenFileActionTitle { get; set; } = "Open File";
        public string OpenWithPdfArchitect { get; private set; } = "Use PDF Architect";
        public string OpenWithDefault { get; private set; } = "Use windows default viewer";
        public string OpenInViewer { get; private set; } = "Open file in viewer.";
        public string OpenViewerDescription { get; private set; } = "The default viewer is determined by Windows. You can set up your own viewer in our viewer section in the settings or change the windows default";
    }
}
