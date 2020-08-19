using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class WorkflowEditorOverlayInteraction : IInteraction
    {
        public bool Success;

        public string Title;
        public string View;

        public WorkflowEditorOverlayInteraction(bool success, string title, string view)
        {
            Success = success;
            Title = title;
            View = view;
        }
    }
}
