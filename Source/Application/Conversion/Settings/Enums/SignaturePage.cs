using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum SignaturePage
    {
        [Translation("First page")]
        FirstPage,

        [Translation("Last page")]
        LastPage,

        [Translation("Custom page")]
        CustomPage
    }
}