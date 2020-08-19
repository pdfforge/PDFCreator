using pdfforge.LicenseValidator.Interface;
using pdfforge.PDFCreator.Core.Services.Logging;
using pdfforge.PDFCreator.ErrorReport;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tartaros;

namespace pdfforge.PDFCreator.Core.Services
{
    public class ErrorReportHelper
    {
        private readonly InMemoryLogger _inMemoryLogger;
        private readonly IVersionHelper _versionHelper;
        private readonly IAssemblyHelper _assemblyHelper;

        public ErrorReportHelper(InMemoryLogger inMemoryLogger, IVersionHelper versionHelper, IAssemblyHelper assemblyHelper)
        {
            _inMemoryLogger = inMemoryLogger;
            _versionHelper = versionHelper;
            _assemblyHelper = assemblyHelper;
        }

        public static ILicenseChecker LicenseChecker { private get; set; }

        private Dictionary<string, string> BuildAdditionalEntries()
        {
            var additionalEntries = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(Thread.CurrentThread.Name))
                additionalEntries["Thread"] = Thread.CurrentThread.Name;

            if (LicenseChecker == null)
                return additionalEntries;

            var activation = LicenseChecker.GetSavedActivation();
            activation
                .MatchSome(a =>
            {
                additionalEntries["LicenseKey"] = a.Key;
                additionalEntries["MachineID"] = a.MachineId;
            });

            return additionalEntries;
        }

        private ErrorAssistant CreateErrorAssistant()
        {
            return new ErrorAssistant("pdfcreator", _versionHelper.ApplicationVersion);
        }

        private Report CreateReport(ErrorAssistant errorAssistant, Exception ex)
        {
            var report = errorAssistant.BuildReport(ex, BuildAdditionalEntries());

            report.Log.AddRange(_inMemoryLogger.LogEntries);

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

            var errorAssistant = CreateErrorAssistant();
            var report = CreateReport(errorAssistant, ex);

            errorAssistant.ShowErrorWindow(report);
        }

        public void ShowErrorReportInNewProcess(Exception ex)
        {
            if (IsIgnoredException(ex))
                return;

            var errorAssistant = CreateErrorAssistant();
            var report = CreateReport(errorAssistant, ex);

            var errorReporterPath = _assemblyHelper.GetAssemblyDirectory();
            errorReporterPath = Path.Combine(errorReporterPath, "ErrorReport.exe");

            if (!File.Exists(errorReporterPath))
                return;

            try
            {
                var errorFile = Path.GetTempPath() + Guid.NewGuid() + ".err";
                errorAssistant.SaveReport(report, errorFile);
                Process.Start(errorReporterPath, "\"" + errorFile + "\"");
            }
            catch
            {
            }
        }
    }
}
