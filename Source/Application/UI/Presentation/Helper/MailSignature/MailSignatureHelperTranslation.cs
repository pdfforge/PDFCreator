using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class MailSignatureHelperTranslation : ITranslatable
    {
        public string MailSignatureFreeVersion { get; private set; } = "Email automatically created by the free PDFCreator";
        public string MailSignatureLicensed { get; private set; } = "Email automatically created by PDFCreator";
    }
}
