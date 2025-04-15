using System.Threading.Tasks;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public class ConditionalHintManagerDisabled : IConditionalHintManager
    {
        public int CurrentJobCounter => 0;

        public bool ShouldProfessionalHintBeDisplayed()
        {
            return false;
        }

        public bool ShouldEmailCollectionHintBeDisplayed()
        {
            return false;
        }

        public Task<bool> SendEmailInformation(string emailAddress, bool marketingConsent)
        {
            return Task.FromResult(false);
        }
    }
}
