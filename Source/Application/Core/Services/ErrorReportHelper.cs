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
    public interface IErrorReportHelper
    {
        void ShowErrorReport(Exception ex);
        void ShowErrorReportInNewProcess(Exception ex);
        ErrorHelper ErrorHelper { get; set; }
        void SetLicenseChecker(ILicenseChecker licenseChecker);
    }

    public class ErrorReportHelper : IErrorReportHelper
    {
        private readonly InMemoryLogger _inMemoryLogger;
        private readonly IAssemblyHelper _assemblyHelper;
        private static ILicenseChecker _licenseChecker;
        private static IErrorReportHelper _instance;
        public ErrorHelper ErrorHelper { get; set; }

        private ErrorReportHelper(InMemoryLogger inMemoryLogger, IAssemblyHelper assemblyHelper, ErrorHelper errorHelper)
        {
            _inMemoryLogger = inMemoryLogger;
            _assemblyHelper = assemblyHelper;
            ErrorHelper = errorHelper;
        }

        public static IErrorReportHelper GetInstance(InMemoryLogger inMemoryLogger, IAssemblyHelper assemblyHelper, ErrorHelper errorHelper)
        {
            return _instance ??= new ErrorReportHelper(inMemoryLogger, assemblyHelper, errorHelper);
        }

        public static IErrorReportHelper GetInstance()
        {
            return _instance;
        }

        public void SetLicenseChecker(ILicenseChecker licenseChecker)
        {
            _licenseChecker = licenseChecker;
        }

        private Dictionary<string, string> BuildAdditionalEntries()
        {
            var additionalEntries = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(Thread.CurrentThread.Name))
                additionalEntries[SentryTagNames.ThreadName] = Thread.CurrentThread.Name;

            if (_licenseChecker == null)
                return additionalEntries;

            var activation = _licenseChecker.GetSavedActivation();
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
