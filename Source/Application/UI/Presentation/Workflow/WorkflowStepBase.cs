using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public abstract class WorkflowStepBase : IWorkflowStep
    {
        public abstract string NavigationUri { get; }

        public abstract bool IsStepRequired(Job job);

        public Task ExecuteStep(Job job, IWorkflowViewModel workflowViewModel)
        {
            return workflowViewModel.ExecuteWorkflowStep(job);
        }
    }
}
