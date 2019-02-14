using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome
{
    public class WelcomeWindowTranslation : ITranslatable
    {
        public string Title { get; private set; } = "Welcome";
        private string WelcomeTextHeadlineText { get; set; } = "Thank you for installing PDFCreator {0}.";
        public string WhatsNewButtonContent { get; private set; } = "What's new?";
        public string PrioritySupport { get; private set; } = "Priority Support";

        public string GetWelcomeText(string editionNameWithVersion)
        {
            return string.Format(WelcomeTextHeadlineText, editionNameWithVersion);
        }
    }
}
