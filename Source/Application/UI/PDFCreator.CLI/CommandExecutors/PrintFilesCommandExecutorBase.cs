using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemInterface.IO;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public abstract class PrintFilesCommandExecutorBase : ICommandExecutor
    {
        protected abstract IFile File { get; }
        protected abstract IPrintFileHelper PrintFileHelper { get; }
        protected abstract ISettingsManager SettingsManager { get; }

        public abstract void InitializeDependencies();

        public CheckResult IsExecutable() => CheckResult.Success();

        protected abstract IList<string> GetFiles();
        protected abstract string GetPrinter();

        protected abstract void PrePrintAction();

        public Task<CommandResult> Execute()
        {
            var files = GetFiles();

            var firstNonExistingFile = files.FirstOrDefault(f => !File.Exists(f));

            if (firstNonExistingFile != null)
            {
                Console.WriteLine("The file \"{0}\" does not exist!", firstNonExistingFile);
                return Task.FromResult(CommandResult.Error(ExitCode.PrintFileDoesNotExist, $"The file \"{firstNonExistingFile}\" does not exist"));
            }

            SettingsManager.LoadAllSettings();

            PrintFileHelper.PdfCreatorPrinter = GetValidPrinterName(GetPrinter());

            foreach (var file in files)
            {
                if (!PrintFileHelper.AddFile(file, true))
                {
                    return Task.FromResult(CommandResult.Error(ExitCode.PrintFileNotPrintable, $"The file \"{file}\" is not printable"));
                }
            }

            PrePrintAction();

            if (!PrintFileHelper.PrintAll(true))
            {
                return Task.FromResult(CommandResult.Error(ExitCode.PrintFileCouldNotBePrinted, "At least one file could not be printed"));
            }

            return Task.FromResult(CommandResult.Success());
        }

        private string GetValidPrinterName(string printerName)
        {
            if (!string.IsNullOrWhiteSpace(printerName))
            {
                var settings = SettingsManager.GetSettingsProvider().Settings;
                if (settings.ApplicationSettings.PrinterMappings.Any(p => p.PrinterName == printerName))
                    return printerName;
            }

            var settingsProvider = SettingsManager.GetSettingsProvider();
            return settingsProvider.Settings.CreatorAppSettings.PrimaryPrinter;
        }
    }
}
