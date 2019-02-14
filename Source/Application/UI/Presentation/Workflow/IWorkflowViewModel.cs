using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public interface IWorkflowViewModel
    {
        Task ExecuteWorkflowStep(Job job);
    }
}
