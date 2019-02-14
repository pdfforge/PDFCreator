using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;

namespace pdfforge.PDFCreator.Core.Printing.Printer
{
    public interface ISystemPrinterProvider
    {
        string GetDefaultPrinter();

        IEnumerable<string> GetInstalledPrinterNames();
    }

    public class SystemPrinterProvider : ISystemPrinterProvider
    {
        public string GetDefaultPrinter()
        {
            var settings = new PrinterSettings();
            return settings.PrinterName;
        }

        public IEnumerable<string> GetInstalledPrinterNames()
        {
            var printers = PrinterSettings.InstalledPrinters.Cast<string>().ToList();
            printers.Sort();
            return printers;
        }
    }
}
