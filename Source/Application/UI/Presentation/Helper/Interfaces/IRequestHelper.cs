using System.Threading.Tasks;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Interfaces;
public interface IRequestHelper
{
    Task<(bool isSuccessful, string interactionMesage)> RequestTrial(RequestHelperTranslation translation, string emailAddress, string product, string trialRequestLink, bool marketingConsent = false, bool isUpdate = false, bool isSetup = false);
}
