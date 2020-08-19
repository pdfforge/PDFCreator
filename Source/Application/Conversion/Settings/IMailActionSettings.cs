using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public interface IMailActionSettings
    {
        bool AddSignature { get; set; }
        List<string> AdditionalAttachments { get; set; }
        string Content { get; set; }
        bool Html { get; set; }
        string Recipients { get; set; }
        string RecipientsBcc { get; set; }
        string RecipientsCc { get; set; }
        string Subject { get; set; }
    }
}
