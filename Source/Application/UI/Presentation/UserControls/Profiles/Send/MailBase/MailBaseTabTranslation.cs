using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.MailBase
{
    public interface IMailBaseTabTranslation
    {
        string AdditionalAttachmentsText { get; set; }
        string SelectAttachmentTitle { get; set; }
        string AllFiles { get; set; }
    }

    public class MailBaseTabTranslation : ITranslatable, IMailBaseTabTranslation
    {
        public string AdditionalAttachmentsText { get; set; } = "Additional E-mail Attachments:";
        public string SelectAttachmentTitle { get; set; } = "Select E-Mail Attachment";
        public string AllFiles { get; set; } = "All Files";
    }
}
