using pdfforge.PDFCreator.Conversion.Settings;
using System.Linq;
using pdfforge.PDFCreator.Core.Printing.Printer;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface IPrinterMappingsHelper
    {
        void CheckPrinterMappings(PdfCreatorSettings settings);
    }

    public class PrinterMappingsHelper : IPrinterMappingsHelper
    {
        private readonly IPrinterHelper _printerHelper;

        public PrinterMappingsHelper(IPrinterHelper printerHelper)
        {
            _printerHelper = printerHelper;
        }
        
        public void CheckPrinterMappings(PdfCreatorSettings settings)
        {
            var printers = _printerHelper.GetPDFCreatorPrinters();

            // if there are no printers, something is broken and we need to fix that first
            if (!printers.Any())
                return;

            //Assign DefaultProfile for all installed printers without mapped profile.
            foreach (var printer in printers)
            {
                if (settings.ApplicationSettings.PrinterMappings.All(o => o.PrinterName != printer))
                    settings.ApplicationSettings.PrinterMappings.Add(new PrinterMapping(printer,
                        ProfileGuids.DEFAULT_PROFILE_GUID));
            }
            //Remove uninstalled printers from mapping
            foreach (var mapping in settings.ApplicationSettings.PrinterMappings.ToArray())
            {
                if (printers.All(o => o != mapping.PrinterName))
                    settings.ApplicationSettings.PrinterMappings.Remove(mapping);
            }
            //Check primary printer
            if (
                settings.ApplicationSettings.PrinterMappings.All(
                    o => o.PrinterName != settings.CreatorAppSettings.PrimaryPrinter))
            {
                settings.CreatorAppSettings.PrimaryPrinter =
                    _printerHelper.GetApplicablePDFCreatorPrinter("PDFCreator", "PDFCreator") ?? "";
            }
        }
    }
}
