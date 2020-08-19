using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab
{
    public class ActionInfoTextTranslation : ITranslatable
    {
        public string Attachment { get; private set; } = "Appends one or more pages at the end of the document.";
        public string Background { get; private set; } = "Adds a background to the document.";
        public string Cover { get; private set; } = "Inserts one or more pages at the beginning of the document.";
        public string Stamp { get; private set; } = "Places a stamp text on the document.";
        public string Ftp { get; private set; } = "Uploads the document with FTP.";
        public string Email { get; private set; } = "Opens the default e-mail client with the converted document as attachment.";
        public string Smtp { get; private set; } = "Sends the document as an e-mail without user interaction by using SMTP.";
        public string Http { get; private set; } = "Uploads the document to an HTTP server.";
        public string Print { get; private set; } = "Prints the document to a physical printer.";

        public string Script { get; private set; } = "Calls a program or script that further processes the document after the conversion. " +
                                                     "PDFCreator calls the specified program automatically and transfers the created files as parameter.";

        public string CsScript { get; private set; } = "Implement a custom script in C# to process the print job.";

        public string Dropbox { get; private set; } = "Uploads the document to Dropbox to save it or to share it with a link.";
        public string Encryption { get; private set; } = "Secures PDFs with a password and selects which authorizations the user receives. Signature and Encryption are always processed last.";
        public string Signature { get; private set; } = "Digitally signs PDF files, for example to verify the sender’s identity and to ensure the document has not been changed after signing. Signature and Encryption are always processed last.";
        public string UserTokens { get; private set; } = "Extracts values from the source document and uses them anywhere they are supported by PDFCreator.";

        public string ForwardToFurtherProfile { get; private set; } = "Trigger another conversion by forwarding the original source document to an other profile, e.g. to convert to another output format.";

        public string Watermark { get; private set; } = "Adds a watermark to the document.";
    }
}
