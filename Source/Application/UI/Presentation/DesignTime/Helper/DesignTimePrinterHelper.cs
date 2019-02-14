using pdfforge.PDFCreator.Core.Printing.Printer;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimePrinterHelper : IPrinterHelper
    {
        public IList<string> GetPDFCreatorPrinters()
        {
            return new[] { "PDFCreator" };
        }

        public IList<string> GetPrinters(string portName)
        {
            return new[] { "PDFCreator" };
        }

        public string GetApplicablePDFCreatorPrinter(string requestedPrinter)
        {
            return requestedPrinter;
        }

        public string GetApplicablePDFCreatorPrinter(string requestedPrinter, string defaultPrinter)
        {
            return requestedPrinter;
        }

        public void PrintWindowsTestPage(string printerName)
        {
        }

        public string GetDefaultPrinter()
        {
            return "PDFCreator";
        }

        public bool SetDefaultPrinter(string printerName)
        {
            return true;
        }

        public PrinterNameValidation ValidatePrinterName(string printerName)
        {
            return PrinterNameValidation.InvalidName;
        }

        public bool IsValidPrinterName(string printerName)
        {
            return false;
        }

        public string CreateValidPrinterName(string printerBaseName)
        {
            return printerBaseName;
        }
    }
}
