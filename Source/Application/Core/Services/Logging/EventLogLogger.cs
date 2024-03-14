using NLog;
using NLog.Config;
using NLog.Filters;
using NLog.Targets;
using System.Collections.Generic;
using System.Threading;

namespace pdfforge.PDFCreator.Core.Services.Logging
{
    public class EventLogLogger : ILogger
    {
        private const string TraceLogLayout =
            "[${level}] ${processid}-${threadid} (${threadname}) ${callsite}: ${message} ${exception:innerFormat=type,message:maxInnerExceptionLevel=1:format=tostring}";

        private const string ShortLogLayout =
            "[${level}] ${callsite}: ${message} ${exception:innerFormat=type,message:maxInnerExceptionLevel=1:format=tostring}";
        private const string BaseRule = "BaseRule";
        private const string LogCollectorRule = "LogCollectorRule";

        private readonly string _logSourceName;
        private readonly string _logName;
        private readonly PerThreadLogCollector _logCollector;
        private readonly List<LoggingRule> _loggingRules = new List<LoggingRule>();
        
        private EventLogTarget BuildEventLog(string name, string layout)
        {
            var eventLogTarget = new EventLogTarget();

            eventLogTarget.Name = name;
            eventLogTarget.Log = _logName;
            eventLogTarget.Source = _logSourceName;
            eventLogTarget.MachineName = ".";
            eventLogTarget.Layout = layout;

            return eventLogTarget;
        }

        public EventLogLogger(string logSourceName, string logName, LogLevel logLevel, PerThreadLogCollector logCollector)
        {
            _logSourceName = logSourceName;
            _logName = logName;
            _logCollector = logCollector;

            LogManager.Configuration = new LoggingConfiguration();

            ApplyLoggingRules(GetLoggingRules(logLevel));
        }

        private IEnumerable<LoggingRule> GetLoggingRules(LogLevel logLevel)
        {
            var loggingLayout = GetLayoutForLogLevel(logLevel);

            var eventLogTarget = BuildEventLog("EventLogTarget", loggingLayout);
            var baseRule = new LoggingRule("*", logLevel, eventLogTarget);

            yield return baseRule;

            var collectorEventLogTarget = BuildEventLog("CollectorEventLogTarget", "${message}");
            var workerLoggerRule = new LoggingRule(PerThreadLogCollector.WorkerLoggerName, logLevel, collectorEventLogTarget);

            yield return workerLoggerRule;

            var logCollectorRule = new LoggingRule("*", logLevel, _logCollector);
            _logCollector.Layout = loggingLayout;

            foreach (var threadName in _logCollector.AcceptedThreadNames)
            {
                baseRule.Filters.Add(new WhenMethodFilter(_ => ShouldIgnoreLogEvent(BaseRule, threadName)));
                logCollectorRule.Filters.Add(new WhenMethodFilter(_ => ShouldIgnoreLogEvent(LogCollectorRule, threadName)));
            }

            yield return logCollectorRule;
        }

        private static FilterResult ShouldIgnoreLogEvent(string ruleName, string acceptedThreadName)
        {
            var currentThreadName = Thread.CurrentThread.Name;

            if (currentThreadName == acceptedThreadName)
            {
                if(string.Equals(ruleName, BaseRule))
                    return FilterResult.Ignore;
                if(string.Equals(ruleName, LogCollectorRule))
                    return FilterResult.Log;
            }
            return FilterResult.Log;
        }

        private void ApplyLoggingRules(IEnumerable<LoggingRule> loggingRules)
        {
            var config = LogManager.Configuration;

            foreach (var loggingRule in _loggingRules)
            {
                config.LoggingRules.Remove(loggingRule);
            }

            foreach (var loggingRule in loggingRules)
            {
                config.LoggingRules.Add(loggingRule);
                _loggingRules.Add(loggingRule);
            }

            LogManager.ReconfigExistingLoggers();
        }

        public void ChangeLogLevel(LogLevel logLevel)
        {
            ApplyLoggingRules(GetLoggingRules(logLevel));
        }

        public string GetLogPath()
        {
            return "eventvwr /c:" + _logName;
        }

        private string GetLayoutForLogLevel(LogLevel logLevel)
        {
            return logLevel == LogLevel.Trace ? TraceLogLayout : ShortLogLayout;
        }
    }
}
