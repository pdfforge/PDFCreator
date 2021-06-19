using pdfforge.PDFCreator.Conversion.Jobs;

namespace pdfforge.PDFCreator.Core.Workflow.Queries
{
    public interface IErrorNotifier
    {
        void NotifyWithWindow(ActionResult actionResult);

        void NotifyWithOverlay(ActionResult actionResult);

        void NotifyIgnoredWithWindow(ActionResult actionResult);
    }
}
