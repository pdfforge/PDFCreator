using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.JobInfoQueue;

namespace pdfforge.PDFCreator.Conversion.Actions
{
    public class ForwardToFurtherProfileAction : ForwardToFurtherProfileActionBase
    {
        private readonly IJobInfoQueue _jobInfoQueue;

        public ForwardToFurtherProfileAction(IJobInfoQueue jobInfoQueue, IJobInfoDuplicator jobInfoDuplicator)
        {
            _jobInfoQueue = jobInfoQueue;
            JobInfoDuplicator = jobInfoDuplicator;
        }

        protected override IJobInfoDuplicator JobInfoDuplicator { get; }

        protected override void Enqueue(JobInfo jobInfo)
        {
            _jobInfoQueue.AddFirst(jobInfo);
        }
    }
}
