using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class RestartApplicationInteraction : IInteraction
    {
        public RestartApplicationInteractionResult InteractionResult;
    }

    public enum RestartApplicationInteractionResult
    {
        Cancel,
        Later,
        Now
    }
}
