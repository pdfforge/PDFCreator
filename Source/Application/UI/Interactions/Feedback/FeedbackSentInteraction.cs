using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.UI.Interactions.Enums;

namespace pdfforge.PDFCreator.UI.Interactions.Feedback
{
    public class FeedbackSentInteraction : IInteraction
    {
        public FeedbackType SelectedFeedbackType { get; }
        public FeedbackSentInteraction(FeedbackType selectedFeedbackType)
        {
            SelectedFeedbackType = selectedFeedbackType;
        }
    }
}
