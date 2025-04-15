using NLog;
using pdfforge.PDFCreator.Core.Printing.Port;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using pdfforge.LicenseValidator.Tools.Machine;
using SystemInterface.Microsoft.Win32;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public class ConditionalHintManager : IConditionalHintManager
    {
        private const string RegistryKeyForCounter = "LastPlusHintCounter";
        private const string RegistryKeyForDate = "LastPlusHintDate";
        private const string RegistryKeyForEmailFlag = "EmailSubmitted";

        private const int MinNumberOfJobsTillHint = 100;
        private static readonly TimeSpan MinTimeTillHint = TimeSpan.FromDays(14);
        private static readonly TimeSpan EmailCollectionHintInterval = TimeSpan.FromDays(1);

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPrinterPortReader _portReader;
        private readonly IRegistry _registry;
        private readonly string _registryKeyForHintSettings;
        private readonly IHashUtil _hashUtil;

        public ConditionalHintManager(IPrinterPortReader portReader, IRegistry registry, IInstallationPathProvider installationPathProvider, IHashUtil hashUtil)
        {
            _portReader = portReader;
            _registry = registry;
            _registryKeyForHintSettings = @"HKEY_CURRENT_USER\" + installationPathProvider.ApplicationRegistryPath;
            _hashUtil = hashUtil;
        }

        public int CurrentJobCounter { get; private set; }

        public bool ShouldProfessionalHintBeDisplayed()
        {
            var lastJobCounter = GetLastJobCounter(RegistryKeyForCounter);
            var lastDate = GetLastHintDisplayDate();

            CurrentJobCounter = ReadCurrentJobCounter();

            var jobDelta = CurrentJobCounter - lastJobCounter;
            var timeDelta = DateTime.Now - lastDate;

            if (jobDelta < MinNumberOfJobsTillHint || timeDelta < MinTimeTillHint)
                return false;

            WriteLastHintDisplayDate();
            WriteCounter(RegistryKeyForCounter, CurrentJobCounter);
            return true;
        }

        public bool ShouldEmailCollectionHintBeDisplayed()
        {
            if (IsEmailSubmitted())
                return false;

            var lastHintDisplayDate = GetLastHintDisplayDate();
            var timeSinceLastDisplay = DateTime.Now - lastHintDisplayDate;

            if (timeSinceLastDisplay < EmailCollectionHintInterval)
                return false;

            WriteLastHintDisplayDate();
            return true;
        }

        public async Task<bool> SendEmailInformation(string emailAddress, bool marketingConsent)
        {
            using var client = new HttpClient(); // Since email is only collected once, having a disposable client is fine.
            var machineId = new MachineIdV2Generator().GetMachineId(); 

            var request = new HttpRequestMessage(HttpMethod.Post, Urls.EmailCollectionEndpoint)
            {
                Content = new StringContent(
                    JsonSerializer.Serialize(new
                    {
                        email = emailAddress,
                        marketing_consent = marketingConsent,
                        machine_id = machineId,
                        check = _hashUtil.GetSha256Hash($"{emailAddress}{marketingConsent}{machineId}")
                    }),
                    Encoding.UTF8,
                    "application/json")
            };

            try
            {
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    WriteEmailSubmittedFlag();
                    return true;
                }

                _logger.Error($"Email post request failed with a response: {await response.Content.ReadAsStringAsync()}");
                return false;
            }
            catch (HttpRequestException ex)
            {
                _logger.Error(ex, "Network error while sending email information");
                return false;
            }
            catch (TaskCanceledException ex)
            {
                _logger.Error(ex, "Email collection request timed out");
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error while sending email information");
                return false;
            }
        }

        private DateTime GetLastHintDisplayDate()
        {
            try
            {
                var lastDate = _registry.GetValue(_registryKeyForHintSettings, RegistryKeyForDate, "").ToString();

                if (string.IsNullOrWhiteSpace(lastDate))
                {
                    // Ensuring that it's displayed if no hint was ever displayed before (fresh installation)
                    // and if it's a fresh installation, ProfessionalHintStep would not be displayed (no print jobs yet completed)
                    // so this if condition is only useful for the EmailCollectionHintStep and does not interfere with
                    // ProfessionalHintView's display logic.
                    return DateTime.MinValue;
                }

                var success = DateTime.TryParse(lastDate, out var date);

                return success ? date : DateTime.MinValue;
            }
            catch (NullReferenceException)
            {
                WriteLastHintDisplayDate();
                return DateTime.Now;
            }
        }

        private int GetLastJobCounter(string registryKeyOfCounter)
        {
            try
            {
                var value = _registry.GetValue(_registryKeyForHintSettings, registryKeyOfCounter, 0).ToString();
                if (!int.TryParse(value, out var lastJobCounter))
                    lastJobCounter = 0;

                if (lastJobCounter != 0) return lastJobCounter;

                WriteCounter(registryKeyOfCounter, lastJobCounter);
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

        private void WriteCounter(string registryKey, int counter)
        {
            _registry.SetValue(_registryKeyForHintSettings, registryKey, counter);
        }

        private void WriteLastHintDisplayDate()
        {
            _registry.SetValue(_registryKeyForHintSettings, RegistryKeyForDate, DateTime.Now);
        }

        private bool IsEmailSubmitted()
        {
            try
            {
                var value = _registry.GetValue(_registryKeyForHintSettings, RegistryKeyForEmailFlag, defaultValue: 0).ToString();
                if (!int.TryParse(value, out var isSubmitted))
                    isSubmitted = 0;

                if (isSubmitted == 1)
                    return true;
            }
            catch (Exception e)
            {
                _logger.Error(e, "Could not read EmailSubmittedFlag from registry");
                return false;
            }

            return false;
        }
        private void WriteEmailSubmittedFlag()
        {
            _registry.SetValue(_registryKeyForHintSettings, RegistryKeyForEmailFlag, 1);
        }
    }
}
