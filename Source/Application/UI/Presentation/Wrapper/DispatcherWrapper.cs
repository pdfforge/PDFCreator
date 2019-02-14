using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace pdfforge.PDFCreator.UI.Presentation.Wrapper
{
    public class DispatcherWrapper : IDispatcher
    {
        private readonly Dispatcher _dispatcher;

        public DispatcherWrapper()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void BeginInvoke(Action action)
        {
            _dispatcher.BeginInvoke(action);
        }

        public void BeginInvoke<T>(Action<T> action, T payload)
        {
            _dispatcher.BeginInvoke(action, payload);
        }

        public void BeginInvoke(Action<JobInfo> addMethod, JobInfo jobInfo)
        {
            _dispatcher.BeginInvoke(addMethod, jobInfo);
        }

        public async Task<TResult> InvokeAsync<TResult>(Func<TResult> action)
        {
            return await _dispatcher.InvokeAsync(action);
        }

        public async Task InvokeAsync(Action action)
        {
            await _dispatcher.InvokeAsync(action);
        }
    }
}
