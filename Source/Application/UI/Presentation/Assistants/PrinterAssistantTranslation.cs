using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public class PrinterAssistantTranslation : ITranslatable
    {
        private string AskDeletePrinter { get; set; } = "Are you sure that you want to delete the printer '{0}'?";
        private string CouldNotInstallPrinter { get; set; } = "The printer '{0}' could not be installed.";
        private string CouldNotRenamePrinter { get; set; } = "The printer '{0}' could not be renamed in '{1}'.";
        public string DontDeleteLastPrinter { get; private set; } = "You may not delete the last printer. Uninstall PDFCreator if you really want to remove all related printers.";
        public string DeletePrinter { get; private set; } = "Delete Printer";
        public string EnterPrintername { get; private set; } = "Please enter printer name:";
        private string PrinterAlreadyInstalled { get; set; } = "A printer with the name '{0}' is already installed on your system. Please enter a new printer name:";
        public string InvalidCharsInPrinterName { get; private set; } = "The printer name contains invalid characters.";
        public string InvalidPrinterName { get; private set; } = "The name is invalid or a printer with this name already exists";

        public string GetPrinterAlreadyInstalledMessage(string printerName)
        {
            return string.Format(PrinterAlreadyInstalled, printerName);
        }

        public string GetCouldNotInstallPrinterMessage(string printerName)
        {
            return string.Format(CouldNotInstallPrinter, printerName);
        }

        public string GetCouldNotRenamePrinterMessage(string oldPrinterName, string newPrinterName)
        {
            return string.Format(CouldNotRenamePrinter, oldPrinterName, newPrinterName);
        }

        public string GetAskDeletePrinterMessage(string printerName)
        {
            return string.Format(AskDeletePrinter, printerName);
        }
    }
}
