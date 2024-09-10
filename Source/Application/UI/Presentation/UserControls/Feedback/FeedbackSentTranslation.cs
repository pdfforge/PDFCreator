using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Feedback
{
    public class FeedbackSentTranslation : ITranslatable
    {
        public string WindowTitle { get; private set; } = "Successfully shared";
        public string TitleText { get; private set; } = "Your feedback has been sent!";
        public string PositiveTitleText { get; private set; } = "We value our customers' input!";
        public string BodyText { get; private set; } = "Thank you for taking the time to share your input.";
        public string PositiveBodyText { get; private set; } = "Thank you for sharing your positive experience with us.";
        public string FeatureBodyText { get; private set; } = "Thank you for taking the time to share your ideas.";
        public string NegativeBodyText { get; private set; } = "Thank you for letting us know how we can improve.";
    }
}
