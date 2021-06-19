using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Print
{
    public class PrintTabTranslation : ActionTranslationBase
    {
        public string DisplayName { get; private set; } = "Print document";
        public string DuplexPrintingText { get; private set; } = "Duplex printing (if support by the printer)";
        public string SelectPrinterText { get; private set; } = "Select printer:";
        public string FitToPage { get; private set; } = "Fit to the page size of the printer";

        public EnumTranslation<DuplexPrint>[] DuplexPrintValues { get; set; } = EnumTranslation<DuplexPrint>.CreateDefaultEnumTranslation();
        public EnumTranslation<SelectPrinter>[] SelectPrinterValues { get; set; } = EnumTranslation<SelectPrinter>.CreateDefaultEnumTranslation();

        public string GetPrinterText(SelectPrinter selectPrinter, string printerName)
        {
            if (selectPrinter == SelectPrinter.SelectedPrinter)
                return printerName;

            return SelectPrinterValues[(int)selectPrinter].Translation;
        }

        public override string Title { get; set; } = "Print";
        public override string InfoText { get; set; } = "Prints the document to a physical printer.";

        public string WarnAboutPdfCreatorPrinter { get; private set; } = "The selected printer is mapped to PDFCreator. Please consider using the forwarding action. It doesn't require additional printing, is quicker and less error prone.";
    }
}
