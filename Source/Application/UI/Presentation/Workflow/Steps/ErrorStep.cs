using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class ErrorStep : IErrorStep
    {
        public string NavigationUri => nameof(ErrorView);

        public Task ExecuteStep(Job job, IWorkflowErrorViewModel workflowViewModel, ActionResult error, bool isWarning)
        {
            return workflowViewModel.ExecuteWorkflowStep(job, error, isWarning);
        }
    }
}
