namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OpenFile
{
    public class OpenViewerActionTranslation : ActionTranslationBase
    {
        public string OpenFileActionTitle { get; set; } = "Open File";
        public string OpenWithPdfArchitect { get; private set; } = "Use PDF Architect";
        public string OpenWithDefault { get; private set; } = "Use windows default viewer";
        public string OpenInViewer { get; private set; } = "Open file in viewer.";
        public string OpenViewerDescription { get; private set; } = "The default viewer is determined by Windows. You can set up your own viewer in our viewer section in the settings or change the windows default";
        public override string Title { get; set; } = "Open File";
        public override string InfoText { get; set; } = "Open file in viewer.";
    }
}
