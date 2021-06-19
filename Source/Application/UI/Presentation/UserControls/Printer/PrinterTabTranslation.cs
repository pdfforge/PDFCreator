using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Printer
{
    public class PrinterTabTranslation : ITranslatable
    {
        public string AddPrinterButtonText { get; private set; } = "Add printer";
        public string DeletePrinterButtonText { get; private set; } = "Delete printer";
        public string LastUsedProfileMapping { get; private set; } = "Last used profile";
        public string PrimaryColumnHeader { get; private set; } = "Primary printer";
        public string PrinterColumnHeader { get; private set; } = "Printer";
        public string RenamePrinterButtonText { get; private set; } = "Rename printer";
        public string ManagePrinters { get; private set; } = "Manage Printers";
    }
}
