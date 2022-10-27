using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Printing.Printer;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.CLI.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using SystemInterface.Microsoft.Win32;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public class RestorePrintersExecutor : ICommandExecutor
    {
        private ISharedSettingsLoader _sharedSettingsLoader;
        private IInstallationPathProvider _installationPathProvider;
        private IPrinterProvider _printerProvider;
        private IApplicationLauncher _printerHelperLauncher;
        private IRegistry _registry;

        public void InitializeDependencies()
        {
            var container = BootstrapperHelper.GetConfiguredContainer();
            InitializeDependencies(
                container.GetInstance<ISharedSettingsLoader>(),
                container.GetInstance<IInstallationPathProvider>(),
                container.GetInstance<IPrinterProvider>(),
                container.GetInstance<IRegistry>(),
                new ApplicationLauncher("PrinterHelper.exe", InstallationPathProviders.PDFCreatorProvider)
                );
        }

        public void InitializeDependencies(ISharedSettingsLoader sharedSettingsLoader, IInstallationPathProvider installationPathProvider, IPrinterProvider printerProvider, IRegistry registry, IApplicationLauncher printerHelperLauncher)
        {
            _sharedSettingsLoader = sharedSettingsLoader;
            _installationPathProvider = installationPathProvider;
            _printerProvider = printerProvider;
            _printerHelperLauncher = printerHelperLauncher;
            _registry = registry;
        }

        public CheckResult IsExecutable()
        {
            if (!_printerHelperLauncher.CanLaunchApplication())
                return CheckResult.Error("Could not find PrinterHelper.exe");

            return CheckResult.Success();
        }

        public Task<CommandResult> Execute()
        {
            try
            {
                var allMissingPrinters = new List<string>();
                allMissingPrinters.AddRange(FindMissingPrinters(_sharedSettingsLoader.GetSharedPrinterMappings()));

                var missingPrinters = FindMissingPrinters(GetMappingsFromRegistry());
                allMissingPrinters.AddRange(missingPrinters.Except(allMissingPrinters));

                if (!allMissingPrinters.Any())
                {
                    Console.WriteLine("The expected printers already exist");
                    return Task.FromResult(CommandResult.Success());
                }

                Console.WriteLine("Adding missing printers: " + string.Join(", ", allMissingPrinters));
                return AddPrinters(allMissingPrinters);
            }
            catch (Exception ex)
            {
                return Task.FromResult(CommandResult.Error(ExitCode.Unknown, "There was an error while restoring the printers: " + ex.Message));
            }
        }

        private async Task<CommandResult> AddPrinters(IList<string> printers)
        {
            var args = new List<string>();
            args.Add("addPrinter");
            args.Add("/SinglePort");
            args.Add("/Name=" + string.Join(",", printers));

            try
            {
                var exitCode = await _printerHelperLauncher.LaunchApplication(args.ToArray());
                if (exitCode == 0)
                    return CommandResult.Success();

                return CommandResult.Error(ExitCode.PrintersBroken, "PrinterHelper.exe ended with exit code " + exitCode);
            }
            catch (Exception ex)
            {
                return CommandResult.Error(ExitCode.PrintersBroken, "Could not start PrinterHelper.exe: " + ex.Message);
            }
        }

        private IEnumerable<PrinterMapping> GetMappingsFromRegistry()
        {
            var printerMappingsKey = Path.Combine(_installationPathProvider.SettingsRegistryPath, "ApplicationSettings\\PrinterMappings");
            var printerList = new List<PrinterMapping>();
            var printerRegKey = _registry.CurrentUser.OpenSubKey(printerMappingsKey);
            var numMappingsString = (string)printerRegKey.GetValue("numClasses");

            if (!int.TryParse(numMappingsString, out var numMappings))
                return new PrinterMapping[0];

            for (int i = 0; i < numMappings; i++)
            {
                var key = _registry.CurrentUser.OpenSubKey(Path.Combine(printerMappingsKey, i.ToString()));
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
