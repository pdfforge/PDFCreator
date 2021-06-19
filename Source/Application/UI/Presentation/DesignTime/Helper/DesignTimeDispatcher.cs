using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System;
using System.Threading.Tasks;
using static System.Threading.Tasks.Task;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeDispatcher : IDispatcher
    {
        public void BeginInvoke(Action action)
        {
        }

        public void BeginInvoke<T>(Action<T> action, T payload)
        {
        }

        public void BeginInvoke(Action<JobInfo> addMethod, JobInfo jobInfo)
        {
        }

        public Task<TResult> InvokeAsync<TResult>(Func<TResult> action)
        {
            return FromResult(action.Invoke());
        }

        public Task InvokeAsync(Action action)
        {
            return Task.CompletedTask;
        }
    }
}
