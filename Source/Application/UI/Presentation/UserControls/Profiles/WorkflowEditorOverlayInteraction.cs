using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class WorkflowEditorOverlayInteraction : IInteraction
    {
        public WorkflowEditorOverlayResult Result { get; set; }

        public string Title { get; set; }
        public string View { get; }
        public bool IsDisabled { get; }
        public bool ShowBackButton { get; }

        public WorkflowEditorOverlayInteraction(string title, string view, bool isDisabled, bool showBackButton)
        {
            Title = title;
            View = view;
            IsDisabled = isDisabled;
            ShowBackButton = showBackButton;
        }
    }

    public enum WorkflowEditorOverlayResult
    {
        Close,
        Back,
        Success
    }
}
