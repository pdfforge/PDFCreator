using System.Collections.Generic;

namespace pdfforge.PDFCreator.Core.Printing
{
    public interface IRepairPrinterAssistant
    {
        bool TryRepairPrinter(IEnumerable<string> printerNames);

        bool IsRepairRequired();
    }
}
