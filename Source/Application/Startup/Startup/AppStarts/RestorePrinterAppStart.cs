using Microsoft.Win32;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class RestorePrinterAppStart : AppStartBase
    {
        private readonly IUacAssistant _uacAssistant;
        private readonly IPrinterProvider _printerProvider;
        private readonly IInstallationPathProvider _installationPathProvider;
        private readonly ISharedSettingsLoader _sharedSettingsLoader;

        public RestorePrinterAppStart(ICheckAllStartupConditions checkAllStartupConditions, IPrinterProvider printerProvider,
            IUacAssistant uacAssistant, IInstallationPathProvider installationPathProvider,
            ISharedSettingsLoader sharedSettingsLoader) : base(checkAllStartupConditions)
        {
            _uacAssistant = uacAssistant;
            _printerProvider = printerProvider;
            _installationPathProvider = installationPathProvider;
            _sharedSettingsLoader = sharedSettingsLoader;
            SkipStartupConditionCheck = true;
        }

        public override Task<ExitCode> Run()
        {
            try
            {
                var allMissingPrinters = new List<string>();
                allMissingPrinters.AddRange(FindMissingPrinters(_sharedSettingsLoader.GetSharedPrinterMappings()));

                var missingPrinters = FindMissingPrinters(GetMappingsFromRegistry());
                allMissingPrinters.AddRange(missingPrinters.Except(allMissingPrinters));
                _uacAssistant.AddPrinters(allMissingPrinters.ToArray());
            }
            catch (Exception)
            {
                return Task.FromResult(ExitCode.Unknown);
            }

            return Task.FromResult(ExitCode.Ok);
        }

        private IEnumerable<PrinterMapping> GetMappingsFromRegistry()
        {
            var printerMappingsKey = Path.Combine(_installationPathProvider.ApplicationRegistryPath, "Settings\\ApplicationSettings\\PrinterMappings");
            var printerList = new List<PrinterMapping>();
            var printerRegKey = Registry.CurrentUser.OpenSubKey(printerMappingsKey);
            var mappingSubKeyNames = printerRegKey?.GetSubKeyNames();

            if (mappingSubKeyNames != null)
                foreach (var subKeyName in mappingSubKeyNames)
                {
                    var key = Registry.CurrentUser.OpenSubKey(Path.Combine(printerMappingsKey, subKeyName));
                    var printerName = key?.GetValue("PrinterName").ToString();
                    var profileGuid = key?.GetValue("ProfileGuid").ToString();
                    if (!string.IsNullOrEmpty(printerName) & !string.IsNullOrEmpty(profileGuid))
                    {
                        var mapping = new PrinterMapping(printerName, profileGuid);
                        printerList.Add(mapping);
                    }
                }

            return printerList;
        }

        private List<string> FindMissingPrinters(IEnumerable<PrinterMapping> printerMappings)
        {
            var installedPrinters = _printerProvider.GetPDFCreatorPrinters();

            return printerMappings
                .Select(pm => pm.PrinterName)
                .Where(p => !installedPrinters.Contains(p))
                .Distinct()
                .ToList();
        }
    }
}
