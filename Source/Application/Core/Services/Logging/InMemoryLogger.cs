using NLog;
using NLog.Config;
using NLog.Targets;
using Sentry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Core.Services.Logging
{
    public class InMemoryLogger : ILogger
    {
        private const string TraceLogLayout =
            "${shortdate} ${date:format=HH\\:mm\\:ss.ffff} [${level}] ${processid}-${threadid} (${threadname}) ${callsite}: ${message}";

        private readonly int _capacity;
        private LoggingRule _loggingRule;

        private FixedMemoryTarget _logTarget;

        public InMemoryLogger(int capacity)
        {
            _capacity = capacity;
        }

        public Breadcrumb[] LogEntries => _logTarget.Logs;

        public void ChangeLogLevel(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public string GetLogPath()
        {
            throw new NotImplementedException();
        }

        public void Register()
        {
            if (_logTarget != null)
                throw new InvalidOperationException("The in-memory logger was already registered");

            var config = LogManager.Configuration ?? new LoggingConfiguration();

            _logTarget = new FixedMemoryTarget(_capacity);
            _logTarget.Layout = TraceLogLayout;

            config.AddTarget("InMemoryTarget", _logTarget);

            _loggingRule = new LoggingRule("*", LogLevel.Trace, _logTarget);
            config.LoggingRules.Add(_loggingRule);

            LogManager.Configuration = config;
        }
    }

    internal sealed class FixedMemoryTarget : TargetWithLayout
    {
        private readonly int _capacity;

        public FixedMemoryTarget(int capacity)
        {
            _capacity = capacity;
        }

        public Breadcrumb[] Logs
        {
            get
            {
                lock (this)
                {
                    return _logs.ToArray();
                }
            }
        }

        private readonly IList<Breadcrumb> _logs = new List<Breadcrumb>();

        private BreadcrumbLevel GetLevel(LogLevel level)
        {
            if (level == LogLevel.Trace)
                return BreadcrumbLevel.Debug;

            if (level == LogLevel.Debug)
                return BreadcrumbLevel.Debug;

            if (level == LogLevel.Warn)
                return BreadcrumbLevel.Warning;

            if (level == LogLevel.Error)
                return BreadcrumbLevel.Error;

            if (level == LogLevel.Fatal)
                return BreadcrumbLevel.Critical;

            return BreadcrumbLevel.Info;
        }

        /// <summary>
        ///     Renders the logging event message and adds it to the internal ArrayList of log messages.
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Write(LogEventInfo logEvent)
        {
            var msg = Layout.Render(logEvent);

            lock (this)
            {
                var breadcrumb = new Breadcrumb(message: logEvent.FormattedMessage, level: GetLevel(logEvent.Level), type: "log");
                _logs.Add(breadcrumb);

                while (_logs.Count > _capacity)
                {
                    _logs.RemoveAt(0);
                }
            }
        }
    }
}
