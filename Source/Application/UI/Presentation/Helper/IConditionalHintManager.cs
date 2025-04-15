using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface IConditionalHintManager
    {
        int CurrentJobCounter { get; }

        bool ShouldProfessionalHintBeDisplayed();
        bool ShouldEmailCollectionHintBeDisplayed();
        Task<bool> SendEmailInformation(string emailAddress, bool marketingConsent);
    }
}
