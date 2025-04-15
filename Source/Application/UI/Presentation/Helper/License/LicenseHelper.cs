using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.Utilities.License;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.License
{
    public class LicenseHelper : ILicenseHelper
    {
        private readonly IInteractionInvoker _interactionInvoker;

        public LicenseHelper(IInteractionInvoker interactionInvoker)
        {
            _interactionInvoker = interactionInvoker;
        }

        public void InformLicenseInteraction()
        {
            _interactionInvoker.Invoke(new LicenseInteraction());
        }
    }
}
