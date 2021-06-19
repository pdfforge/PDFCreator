using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public interface IMailActionSettings : IProfileSetting
    {
        bool AddSignature { get; set; }
        List<string> AdditionalAttachments { get; set; }
        string Content { get; set; }
        EmailFormatSetting Format { get; set; }
        string Recipients { get; set; }
        string RecipientsBcc { get; set; }
        string RecipientsCc { get; set; }
        string Subject { get; set; }

        IMailActionSettings Copy();
    }
}
