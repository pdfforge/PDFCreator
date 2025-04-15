using System.Threading.Tasks;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeConditionalHintManager : IConditionalHintManager
    {
        public int CurrentJobCounter => 101;

        public bool ShouldProfessionalHintBeDisplayed()
        {
            return true;
        }

        public bool ShouldEmailCollectionHintBeDisplayed()
        {
            return true;
        }

        public Task<bool> SendEmailInformation(string emailAddress, bool marketingConsent)
        {
            return Task.FromResult(false);
        }
    }
}
