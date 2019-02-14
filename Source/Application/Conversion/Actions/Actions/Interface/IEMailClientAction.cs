using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Interface
{
    public interface IEMailClientAction : IPostConversionAction
    {
        ActionResult Process(string subject, string body, bool isHtml, bool hasSignature, string signature, string recipientsTo, string recipientsCc, string recipientsBcc, bool hasAttachments, IEnumerable<string> attachedFiles);

        bool CheckEmailClientInstalled();
    }
}
