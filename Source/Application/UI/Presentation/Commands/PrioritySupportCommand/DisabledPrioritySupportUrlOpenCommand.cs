using pdfforge.PDFCreator.Utilities;
using pdfforge.PDFCreator.Utilities.Web;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class DisabledPrioritySupportUrlOpenCommand : UrlOpenCommand, IPrioritySupportUrlOpenCommand
    {
        public DisabledPrioritySupportUrlOpenCommand(IWebLinkLauncher webLinkLauncher, IVersionHelper versionHelper)
            : base(webLinkLauncher)
        {
            Url = "";
        }

        public override bool CanExecute(object parameter)
        {
            return false;
        }
    }
}
