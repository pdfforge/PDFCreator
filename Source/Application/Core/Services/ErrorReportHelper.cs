using pdfforge.LicenseValidator.Interface;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.ErrorReport;
using pdfforge.PDFCreator.Utilities;
using Sentry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Services
{
    public class ErrorReportHelper
    {
        private readonly InMemoryLogger _inMemoryLogger;
        private readonly IAssemblyHelper _assemblyHelper;
        public ErrorHelper ErrorHelper { get; set; }

        public ErrorReportHelper(InMemoryLogger inMemoryLogger, IAssemblyHelper assemblyHelper, ErrorHelper errorHelper)
        {
            _inMemoryLogger = inMemoryLogger;
            _assemblyHelper = assemblyHelper;
            ErrorHelper = errorHelper;
        }

        public static ILicenseChecker LicenseChecker { private get; set; }

        private Dictionary<string, string> BuildAdditionalEntries()
        {
            var additionalEntries = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(Thread.CurrentThread.Name))
                additionalEntries[SentryTagNames.ThreadName] = Thread.CurrentThread.Name;

            if (LicenseChecker == null)
                return additionalEntries;

            var activation = LicenseChecker.GetSavedActivation();
            activation
                .MatchSome(a =>
            {
                additionalEntries[SentryTagNames.LicenseKey] = a.Key;
                additionalEntries[SentryTagNames.MachineId] = a.MachineId;
            });

            return additionalEntries;
        }

        private SentryEvent CreateReport(ErrorHelper errorHelper, Exception ex)
        {
            var report = errorHelper.BuildReport(ex, BuildAdditionalEntries());

            foreach (var logEntry in _inMemoryLogger.LogEntries)
            {
                report.AddBreadcrumb(logEntry);
            }

            return report;
        }

        private bool IsIgnoredException(Exception ex)
        {
            if (ex is TaskCanceledException && ex.StackTrace.Contains("MS.Internal.WeakEventTable.OnShutDown()"))
                return true;

            return false;
        }

        public void ShowErrorReport(Exception ex)
        {
            if (IsIgnoredException(ex))
                return;

            var report = CreateReport(ErrorHelper, ex);

            var assistant = new ErrorAssistant();
            assistant.ShowErrorWindow(report, ErrorHelper);
        }

        public void ShowErrorReportInNewProcess(Exception ex)
        {
            if (IsIgnoredException(ex))
                return;

            var report = CreateReport(ErrorHelper, ex);

            var errorReporterPath = _assemblyHelper.GetAssemblyDirectory();
            errorReporterPath = Path.Combine(errorReporterPath, "ErrorReport.exe");

            if (!File.Exists(errorReporterPath))
                return;

            try
            {
                var errorFile = Path.GetTempPath() + Guid.NewGuid() + ".err";
                ErrorHelper.SaveReport(report, errorFile);
                var arguments = "\"" + errorFile + "\"" + " " + "\"" + ErrorHelper.SentryDsnUrl + "\"";
                Process.Start(errorReporterPath, arguments);
            }
            catch
            {
            }
        }
    }
}
