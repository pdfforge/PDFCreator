using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace pdfforge.PDFCreator.Core.Services.Logging
{
    public interface ILogCollector
    {
        void WriteAndClearLogs(Thread thread);
    }

    public sealed class PerThreadLogCollector : TargetWithLayout, ILogCollector
    {
        public const string WorkerLoggerName = "WorkerThreadLogger";

        public string[] AcceptedThreadNames { get; }
        private readonly List<PerThreadLog> _logs = new List<PerThreadLog>();

        private readonly Logger _workerLogger = LogManager.GetLogger(WorkerLoggerName);

        public PerThreadLogCollector(params string[] acceptedThreadNames)
        {
            AcceptedThreadNames = acceptedThreadNames;
        }

        private PerThreadLog GetOrCreateLog()
        {
            return GetOrCreateLog(Thread.CurrentThread.ManagedThreadId);
        }

        private PerThreadLog GetOrCreateLog(int threadId)
        {
            var log = _logs.FirstOrDefault(l => l?.ThreadId == threadId);

            if (log != null)
                return log;

            log = new PerThreadLog(threadId);
            _logs.Add(log);

            return log;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (!AcceptedThreadNames.Contains(Thread.CurrentThread.Name) || logEvent.LoggerName == WorkerLoggerName)
                return;

            var msg = Layout.Render(logEvent);

            lock (this)
            {
                var log = GetOrCreateLog();
                log.AddLog(logEvent.Level, msg);
            }
        }

        public void WriteAndClearLogs(Thread thread)
        {
            var log = GetAndResetLogs(thread);

            if (!log.Logs.Any())
                return;

            var message = "\r\n" + string.Join("\r\n", log.Logs);

            _workerLogger.Log(log.OverallSeverity, message);
        }

        private PerThreadLog GetAndResetLogs(Thread thread)
        {
            lock (this)
            {
                var log = GetOrCreateLog(thread.ManagedThreadId);
                _logs.Remove(log);
                return log;
            }
        }
    }

    public class PerThreadLog
    {
        private readonly List<string> _logs = new List<string>();

        public PerThreadLog(int threadId)
        {
            ThreadId = threadId;
            CreatedAt = DateTime.Now;
        }

        public int ThreadId { get; }
        public DateTime CreatedAt { get; }
        public IEnumerable<string> Logs => _logs.ToArray();
        public LogLevel OverallSeverity { get; private set; } = LogLevel.Off;

        public void AddLog(LogLevel severity, string msg)
        {
            _logs.Add(msg);

            if (severity > OverallSeverity || OverallSeverity == LogLevel.Off)
                OverallSeverity = severity;
        }
    }
}
