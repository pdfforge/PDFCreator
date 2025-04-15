using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Core.Workflow.Exceptions
{
    public interface IFailedJobHandler
    {
        void HandleFailedJob(Job job, ErrorCode? errorCode);
    }
}
