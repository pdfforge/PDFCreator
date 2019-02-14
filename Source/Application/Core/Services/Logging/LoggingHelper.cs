using NLog;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Core.Services.Logging
{
    /// <summary>
    ///     LoggingUtil provides functionality for setting up and managing the logging behavior.
    /// </summary>
    public static class LoggingHelper
    {
        private static ILogger _logger;

        public static string LogFile => _logger?.GetLogPath();

        public static void InitFileLogger(string applicationName, LoggingLevel loggingLevel, string logFilePath = null)
        {
            InitFileLogger(applicationName, GetLogLevel(loggingLevel), logFilePath);
        }

        public static void InitFileLogger(string applicationName, LogLevel logLevel, string logFilePath = null)
        {
            if (_logger == null)
                _logger = new FileLogger(applicationName, logLevel, logFilePath);
        }

        public static void InitEventLogLogger(string source, string logName, LoggingLevel loggingLevel, PerThreadLogCollector logCollector)
        {
            InitEventLogLogger(source, logName, GetLogLevel(loggingLevel), logCollector);
        }

        private static void InitEventLogLogger(string source, string logName, LogLevel logLevel, PerThreadLogCollector logCollector)
        {
            if (_logger == null)
                _logger = new EventLogLogger(source, logName, logLevel, logCollector);
        }

        public static void InitConsoleLogger(string applicationName, LoggingLevel loggingLevel)
        {
            InitConsoleLogger(applicationName, GetLogLevel(loggingLevel));
        }

        private static void InitConsoleLogger(string applicationName, LogLevel logLevel)
        {
            if (_logger == null)
                _logger = new ConsoleLogger(applicationName, logLevel);
        }

        private static void ChangeLogLevel(LogLevel logLevel)
        {
            _logger?.ChangeLogLevel(logLevel);
        }

        public static void ChangeLogLevel(LoggingLevel loggingLevel)
        {
            ChangeLogLevel(GetLogLevel(loggingLevel));
        }

        private static LogLevel GetLogLevel(LoggingLevel loggingLevel)
        {
            return LogLevel.FromOrdinal((int)loggingLevel);
        }
    }
}
