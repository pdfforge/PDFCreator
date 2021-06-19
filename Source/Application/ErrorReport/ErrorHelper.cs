using pdfforge.PDFCreator.ErrorReport.Sentry;
using Sentry;
using Sentry.Extensibility;
using Sentry.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace pdfforge.PDFCreator.ErrorReport
{
    public static class SentryTagNames
    {
        public const string Product = "product.name";
        public const string Edition = "product.edition";
        public const string MachineId = "user.machine-id";
        public const string LicenseKey = "user.license-key";

        public const string UiLanguage = "ui.language";

        public const string ThreadName = "thread-name";
    }

    public class ErrorHelper
    {
        public string SentryDsnUrl { get; }

        private readonly string _productName;
        private readonly string _displayName;
        private readonly SentryOptions _sentryOptions;
        private readonly MainExceptionProcessor _exceptionProcessor;

        public ErrorHelper(string productName, string displayName, Version version, string sentryDsnUrl)
        {
            SentryDsnUrl = sentryDsnUrl;
            _productName = productName;
            _displayName = displayName;

            _sentryOptions = new SentryOptions
            {
                Dsn = sentryDsnUrl,
                Release = version.ToString(),
                ReportAssemblies = false
            };
            _sentryOptions.DisableAppDomainUnhandledExceptionCapture();

            _exceptionProcessor = new MainExceptionProcessor(() => new SentryStackTraceFactory(_sentryOptions));
        }

        public string CurrentUiLanguage { get; set; }

        public SentryClient BuildSentryClient()
        {
            var client = new SentryClient(_sentryOptions);

            return client;
        }

        public string ComposeErrorText(SentryEvent report)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Error Report for {_displayName} {report.Release}");

            if (report.Message?.Formatted != null)
                sb.AppendLine(report.Message?.Formatted);

            var exception = report.SentryExceptions?.FirstOrDefault();
            if (exception != null)
            {
                sb.AppendLine();
                sb.AppendLine("Exception:");
                sb.AppendLine(exception.Type);
                sb.AppendLine(exception.Value);

                var stackFrames =
                    exception.Stacktrace?.Frames
                        .Select(f => f.Function + " " + f.FileName + ":" + f.LineNumber)
                    ?? new string[0];

                sb.AppendLine(string.Join("\r\n", stackFrames));
            }

            sb.AppendLine();
            sb.AppendLine("Platform:");
            sb.AppendLine(report.Platform);

            sb.AppendLine();
            sb.AppendLine("Additional Data:");
            foreach (var additionalEntry in report.Extra)
            {
                sb.AppendLine($"{additionalEntry.Key}={additionalEntry.Value}");
            }
            foreach (var additionalEntry in report.Tags)
            {
                sb.AppendLine($"{additionalEntry.Key}={additionalEntry.Value}");
            }

            sb.AppendLine();
            sb.AppendLine("Log:");
            sb.AppendLine(string.Join("\r\n", report.Breadcrumbs.Select(b => b.Message)));

            return sb.ToString();
        }

        public SentryEvent BuildReport(Exception ex, Dictionary<string, string> additionalEntries)
        {
            SentryEvent report = null;
            // Create report in own thread, which allows us to set the culture to english to (hopefully) receive an english stack trace
            var t = new System.Threading.Thread(() => { report = BuildReportInThread(ex, additionalEntries); });
            t.CurrentUICulture = CultureInfo.InvariantCulture;
            t.Start();
            t.Join();

            return report;
        }

        private SentryEvent BuildReportInThread(Exception ex, Dictionary<string, string> additionalEntries)
        {
            var report = new SentryEvent(ex);

            report.Message = ex.Message;

            report.SentryExceptions = new List<SentryException>(_exceptionProcessor.CreateSentryException(ex));

            var platform = Environment.Is64BitOperatingSystem ? "x64" : "x86";

            report.SetTag(SentryTagNames.Edition, _productName);
            report.SetTag(SentryTagNames.Product, _displayName);

            if (!string.IsNullOrWhiteSpace(CurrentUiLanguage))
                report.SetTag(SentryTagNames.UiLanguage, CurrentUiLanguage);

            report.Platform = platform;

            foreach (var entry in additionalEntries)
            {
                report.SetExtra(entry.Key, entry.Value);
            }

            return report;
        }

        public void SaveReport(SentryEvent report, string reportFile)
        {
            try
            {
                using (var stream = File.OpenWrite(reportFile))
                using (var writer = new Utf8JsonWriter(stream))
                {
                    report.WriteTo(writer);
                }
            }
            catch
            {
                // ignore
            }
        }

        public SentryEvent LoadReport(string reportFile)
        {
            using (var stream = File.OpenRead(reportFile))
            {
                var json = JsonDocument.Parse(stream);
                return SentryEvent.FromJson(json.RootElement);
            }
        }
    }
}
