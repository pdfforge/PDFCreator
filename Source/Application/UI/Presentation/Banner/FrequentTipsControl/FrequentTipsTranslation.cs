using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Banner
{
    public class FrequentTipsTranslation : ITranslatable
    {
        public string UserTokensTitle { get; private set; } = "User Tokens";
        public string UserTokensText { get; private set; } = "User Tokens allow you to predefine information like e-mail address, subject, text and more directly in the document without having to manually opening PDFCreator for each conversion.";

        //------------------
        public string F1HelpTitle { get; private set; } = "Need help?";

        public string F1HelpText { get; private set; } = "Looking for help with a specific feature or setting? Simply hit the F1 button anywhere in PDFCreator and the related help page will open automatically.";

        //-------------------
        public string PDFCreatorOnlineTitle { get; private set; } = "PDFCreator Online";

        public string PDFCreatorOnlineText { get; private set; } = "Have you tried PDFCreator Online? It lets you convert and merge PDFs quickly and easily in your browser.";

        //-------------------
        public string AutoSaveTitle { get; private set; } = "Automatic Saving";

        public string AutoSaveText { get; private set; } = "Enable automatic mode and skip the interaction with PDFCreator. With a few configurations you will be able to create and save your PDFs to your preferred folder by simply printing directly from your document.";

        //-------------------
        public string TemporarySaveTitle { get; private set; } = "Send PDFs \"without\" saving";

        public string TemporarySaveText { get; private set; } = "All you want to do is send your PDF documents by e-mail, ftp or similar and not bother where to save them? PDFCreator has a feature that allows you to save files only temporarily before they automatically get deleted.";

        //-------------------
        public string WorkflowTitle { get; private set; } = "Your individual workflow";

        public string WorkflowText { get; private set; } = "When activating the workflow editor you can choose the actions that fit best to your requirements and adjust their order according to your individual workflow.";

        //-------------------
        public string DropBoxTitle { get; private set; } = "Send large PDFs with Dropbox";

        public string DropBoxText { get; private set; } = "PDFCreator lets you upload your files directly to Dropbox, where you can save them or share them via a link.";

        //-------------------
        public string ForwardToFurtherProfileTitle { get; private set; } = "Forward to another profile";

        public string ForwardToFurtherProfileText { get; private set; } = "PDFCreator can automatically forward the original document to another profile, for example to convert to another format.";
    }
}
