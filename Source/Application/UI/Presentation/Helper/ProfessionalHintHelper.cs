using NLog;
using pdfforge.PDFCreator.Core.Printing.Port;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;
using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class ProfessionalHintHelper : IProfessionalHintHelper
    {
        private const string RegistryKeyForCounter = "LastPlusHintCounter";
        private const string RegistryKeyForDate = "LastPlusHintDate";

        private const int MinNumberOfJobsTillHint = 100;
        private static readonly TimeSpan MinTimeTillHint = TimeSpan.FromDays(14);

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPrinterPortReader _portReader;
        private readonly IRegistry _registry;
        private readonly string _registryKeyForHintSettings;

        public ProfessionalHintHelper(IPrinterPortReader portReader, IRegistry registry, IInstallationPathProvider installationPathProvider)
        {
            _portReader = portReader;
            _registry = registry;
            _registryKeyForHintSettings = @"HKEY_CURRENT_USER\" + installationPathProvider.ApplicationRegistryPath;
        }

        public int CurrentJobCounter { get; private set; }

        public bool QueryDisplayHint()
        {
            var lastJobCounter = GetLastJobCounter();
            var lastDate = GetLastDate();

            CurrentJobCounter = ReadCurrentJobCounter();

            var jobDelta = CurrentJobCounter - lastJobCounter;
            var timeDelta = DateTime.Now - lastDate;

            if (jobDelta < MinNumberOfJobsTillHint || timeDelta < MinTimeTillHint)
                return false;

            WriteCurrentDate();
            WriteCounter(CurrentJobCounter);
            return true;
        }

        private DateTime GetLastDate()
        {
            try
            {
                var lastDate = _registry.GetValue(_registryKeyForHintSettings, RegistryKeyForDate, "").ToString();

                if (string.IsNullOrWhiteSpace(lastDate))
                {
                    WriteCurrentDate();
                    return DateTime.Now;
                }

                DateTime date;
                var success = DateTime.TryParse(lastDate, out date);

                return success ? date : DateTime.Now;
            }
            catch (NullReferenceException)
            {
                WriteCurrentDate();
                return DateTime.Now;
            }
        }

        private int GetLastJobCounter()
        {
            try
            {
                var value = _registry.GetValue(_registryKeyForHintSettings, RegistryKeyForCounter, 0).ToString();
                if (!int.TryParse(value, out var lastJobCounter))
                    lastJobCounter = 0;

                if (lastJobCounter != 0) return lastJobCounter;

                WriteCounter(lastJobCounter);
                return lastJobCounter;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private int ReadCurrentJobCounter()
        {
            int currentCounter;
            try
            {
                var port = _portReader.ReadPrinterPort("pdfcmon");
                currentCounter = port.JobCounter;
            }
            catch (Exception e)
            {
                currentCounter = 0;
                _logger.Error(e, "Could not read CurrentJobCounter from registry");
            }

            return currentCounter;
        }

        private void WriteCounter(int counter)
        {
            _registry.SetValue(_registryKeyForHintSettings, RegistryKeyForCounter, counter);
        }

        private void WriteCurrentDate()
        {
            _registry.SetValue(_registryKeyForHintSettings, RegistryKeyForDate, DateTime.Now);
        }
    }
}
