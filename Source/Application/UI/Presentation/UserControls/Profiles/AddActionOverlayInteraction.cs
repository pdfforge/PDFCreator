using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class AddActionOverlayInteraction : IInteraction
    {
        public bool Success;

        public AddActionOverlayInteraction(bool success)
        {
            Success = success;
        }
    }
}
