using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum SelectPrinter
    {
        [Translation("Default printer")]
        DefaultPrinter,
        [Translation("Select printer before printing")]
        ShowDialog,
        [Translation("Select printer ->")]
        SelectedPrinter
    }
}