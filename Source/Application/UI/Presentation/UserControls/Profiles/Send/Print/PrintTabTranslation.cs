using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Linq;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Send.Print
{
    public class PrintTabTranslation : ITranslatable
    {
        public string DisplayName { get; private set; } = "Print document";
        public string DuplexPrintingText { get; private set; } = "Duplex printing (if support by the printer)";
        public string SelectPrinterText { get; private set; } = "Select printer:";
        public string Print { get; private set; } = "Print";

        public EnumTranslation<DuplexPrint>[] DuplexPrintValues { get; set; } = EnumTranslation<DuplexPrint>.CreateDefaultEnumTranslation();
        public EnumTranslation<SelectPrinter>[] SelectPrinterValues { get; set; } = EnumTranslation<SelectPrinter>.CreateDefaultEnumTranslation();

        public string GetPrinterText(SelectPrinter selectPrinter, string printerName)
        {
            if (selectPrinter == SelectPrinter.SelectedPrinter)
                return printerName;

            return SelectPrinterValues[(int)selectPrinter].Translation;
        }
    }
}
