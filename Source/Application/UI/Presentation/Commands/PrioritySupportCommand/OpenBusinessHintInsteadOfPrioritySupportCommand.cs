using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Web;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class OpenBusinessHintInsteadOfPrioritySupportCommand : UrlOpenCommand, IPrioritySupportUrlOpenCommand
    {
        public OpenBusinessHintInsteadOfPrioritySupportCommand(IWebLinkLauncher webLinkLauncher)
            : base(webLinkLauncher)
        {
            Url = Urls.BusinessHintLink;
        }
    }
}
