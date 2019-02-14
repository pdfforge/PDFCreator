using System;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Conversion.Jobs
{
    public interface IDispatcher
    {
        void BeginInvoke(Action action);

        void BeginInvoke<T>(Action<T> action, T payload);

        void BeginInvoke(Action<JobInfo.JobInfo> addMethod, JobInfo.JobInfo jobInfo);

        Task<TResult> InvokeAsync<TResult>(Func<TResult> action);

        Task InvokeAsync(Action action);
    }
}
