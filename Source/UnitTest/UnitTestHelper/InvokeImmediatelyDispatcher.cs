using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UnitTest.UnitTestHelper
{
    public class InvokeImmediatelyDispatcher : IDispatcher
    {
        public void BeginInvoke(Action action)
        {
            action();
        }

        public void BeginInvoke<T>(Action<T> action, T payload)
        {
            Task.Run(() => action.Invoke(payload));
        }

        public void BeginInvoke(Action<JobInfo> addMethod, JobInfo jobInfo)
        {
            addMethod(jobInfo);
        }

        public Task<TResult> InvokeAsync<TResult>(Func<TResult> action)
        {
            return Task.Run(action);
        }

        public Task InvokeAsync(Action action)
        {
            return Task.Run(action);
        }
    }

    public class InvokeImmediatelyDispatcherFactory : IDispatcherFactory
    {
        public IDispatcher CreateDispatcher()
        {
            return new InvokeImmediatelyDispatcher();
        }
    }
}
