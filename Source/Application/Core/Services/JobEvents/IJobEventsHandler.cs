using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;

namespace pdfforge.PDFCreator.Core.Services.JobEvents
{
    public interface IJobEventsHandler
    {
        void HandleJobStarted(Job job, string currentThreadName);

        void HandleJobCompleted(Job job, TimeSpan duration);

        void HandleJobFailed(Job job, TimeSpan duration, FailureReason reason);
    }
}
