using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public interface IErrorStep
    {
        string NavigationUri { get; }

        Task ExecuteStep(Job job, IWorkflowErrorViewModel workflowViewModel, ActionResult error, bool isWarning);
    }
}
