using Optional;
using pdfforge.LicenseValidator.Interface.Data;

namespace pdfforge.PDFCreator.Utilities.Trial
{
    public interface ITrialAssistant
    {
        void TrialExpiredMessage(Option<Activation, LicenseError> activation);
    }
}
