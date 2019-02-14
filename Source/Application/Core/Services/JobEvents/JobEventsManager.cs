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
        private readonly List<IJobEventsHandler> _eventHandlers;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public JobEventsManager(IEnumerable<IJobEventsHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers.ToList();
        }

        public void RaiseJobStarted(Job job, string currentThreadName)
        {
            _eventHandlers.ForEach(handler => LogOnException(() => handler.HandleJobStarted(job, currentThreadName)));
        }

        public void RaiseJobCompleted(Job job, TimeSpan duration)
        {
            _eventHandlers.ForEach(handler => LogOnException(() => handler.HandleJobCompleted(job, duration)));
        }

        public void RaiseJobFailed(Job job, TimeSpan duration, FailureReason reason)
        {
            _eventHandlers.ForEach(handler => LogOnException(() => handler.HandleJobFailed(job, duration, reason)));
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
