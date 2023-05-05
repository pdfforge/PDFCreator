using NLog;
using NLog.Targets;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System.IO;
using System.Linq;

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

        public static bool ClearLogFile()
        {
            var config = LogManager.Configuration;
            var fileTarget = config.AllTargets.OfType<FileTarget>().FirstOrDefault();

            if (fileTarget == null)
                return false;

            fileTarget.KeepFileOpen = false;
            LogManager.Configuration = config;

            try
            {
                File.WriteAllText(LogFile, "");
            }
            catch
            {
                return false;
            }

            fileTarget.KeepFileOpen = true;
            LogManager.Configuration = config;

            return true;
        }
    }
}
