namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.OpenFile
{
    public class OpenViewerActionTranslation : ActionTranslationBase
    {
        public string OpenFileActionTitle { get; set; } = "Open file";
        public string OpenWithPdfArchitect { get; private set; } = "Use PDF Architect";
        private string OpenWithCustomViewer { get; set; } = "Use {0}";
        public string OpenWithDefault { get; private set; } = "Use Windows default viewer";
        public string OpenViewerDescription { get; private set; } = "The default viewer is determined by Windows. You can set up your own viewer in the Viewer section of the settings in PDFCreator or change your default viewer for Windows.";
        public override string Title { get; set; } = "Open file";
        public override string InfoText { get; set; } = "Open file in viewer.";
        private string EditorMoreInfo { get; set; } = "Click here for more info on {0}";

        public string FormatOpenWithCustomViewer(string viewerName)
        {
            return string.Format(OpenWithCustomViewer, viewerName);
        }

        public string FormatEditorMoreInfo(string viewerName)
        {
            return string.Format(EditorMoreInfo, viewerName);
        }
    }
}
