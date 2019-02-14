using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Banner
{
    public class FrequentTipsTranslation : ITranslatable
    {
        public string UserTokensTitle { get; private set; } = "User Tokens";
        public string UserTokensText { get; private set; } = "User Tokens allow you to predefine information like e-mail address, subject, text and more directly in the document without having to manually opening PDFCreator for each conversion.";

        //------------------
        public string F1HelpTitle { get; private set; } = "Help is near";

        public string F1HelpText { get; private set; } = "Looking for help with a specific feature or setting? Simply hit the F1 button anywhere in PDFCreator and the related help page will open automatically.";

        //-------------------
        public string PDFCreatorOnlineTitle { get; private set; } = "PDFCreator Online";

        public string PDFCreatorOnlineText { get; private set; } = "Have you tried PDFCreator Online? It lets you convert and merge PDFs quickly and easily in your browser.";

        //-------------------
        public string AutoSaveTitle { get; private set; } = "Automatic Saving";

        public string AutoSaveText { get; private set; } = "Enable automatic mode and skip the interaction with PDFCreator. With a bit of configuration you will be able to create and save your PDFs to your preferred folder by simply printing directly from your document.";
    }
}
