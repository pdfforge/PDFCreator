using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Core.Services.JobEvents
{
    public interface IJobEventsManager
    {
        void RaiseJobStarted(Job job, string currentThreadName);

        void RaiseJobCompleted(Job job, TimeSpan duration);

        void RaiseJobFailed(Job job, TimeSpan duration, FailureReason reason);
    }

    public class JobEventsManager : IJobEventsManager
    {
        private readonly Lazy<List<IJobEventsHandler>> _eventHandlers;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public JobEventsManager(IEnumerable<IJobEventsHandler> eventHandlers)
        {
            // expand IEnumerable on first use to prevent lifetime issues
            _eventHandlers = new Lazy<List<IJobEventsHandler>>(eventHandlers.ToList);
        }

        public void RaiseJobStarted(Job job, string currentThreadName)
        {
            _eventHandlers.Value.ForEach(handler => LogOnException(() => handler.HandleJobStarted(job, currentThreadName)));
        }

        public void RaiseJobCompleted(Job job, TimeSpan duration)
        {
            _eventHandlers.Value.ForEach(handler => LogOnException(() => handler.HandleJobCompleted(job, duration)));
        }

        public void RaiseJobFailed(Job job, TimeSpan duration, FailureReason reason)
        {
            _eventHandlers.Value.ForEach(handler => LogOnException(() => handler.HandleJobFailed(job, duration, reason)));
        }

        private void LogOnException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Error while handling job events!");
            }
        }
    }
}
