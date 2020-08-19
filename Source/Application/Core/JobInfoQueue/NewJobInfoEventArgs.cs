using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System;

namespace pdfforge.PDFCreator.Core.JobInfoQueue
{
    /// <summary>
    ///     EventArgs class that contains the new JobInfo
    /// </summary>
    public class NewJobInfoEventArgs : EventArgs
    {
        public NewJobInfoEventArgs(JobInfo jobInfo)
        {
            JobInfo = jobInfo;
        }

        public JobInfo JobInfo { get; private set; }
    }
}
