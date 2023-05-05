using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum PageSize
    {
        [Translation("Automatic")]
        Automatic = 0,
        [Translation("A4")]
        A4 = 1,
        [Translation("Letter")]
        Letter = 2,
        [Translation("Legal")]
        Legal = 3,
    }
}
