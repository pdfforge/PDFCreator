using Microsoft.Win32;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class RestorePrinterAppStart : AppStartBase
    {
        private readonly IUacAssistant _uacAssistant;
        private readonly IPrinterProvider _printerProvider;
        private readonly IInstallationPathProvider _installationPathProvider;

        public RestorePrinterAppStart(ICheckAllStartupConditions checkAllStartupConditions, IPrinterProvider printerProvider, IUacAssistant uacAssistant, IInstallationPathProvider installationPathProvider) : base(checkAllStartupConditions)
        {
            _uacAssistant = uacAssistant;
            _printerProvider = printerProvider;
            _installationPathProvider = installationPathProvider;
        }

        public override Task<ExitCode> Run()
        {
            try
            {
                var missingPrinters = FindMissingPrinters(GetMappingsFromRegistry());
                _uacAssistant.AddPrinters(missingPrinters.ToArray());
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
