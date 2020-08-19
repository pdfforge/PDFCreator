using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.ActionsInterface
{
    public interface ICheckable : IAction
    {
        void ApplyPreSpecifiedTokens(Job job);

        ActionResult Check(ConversionProfile profile, Accounts accounts, CheckLevel checkLevel);
    }

    public enum CheckLevel
    {
        Profile,
        Job
    }
}
