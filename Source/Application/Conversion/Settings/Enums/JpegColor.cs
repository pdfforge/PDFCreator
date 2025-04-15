using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]

    public enum JpegColor
    {
        [Translation("16 million colors (24 Bit)")]

        Color24Bit,
        [Translation("Grayscale (8 Bit)")]

        Gray8Bit
    }
}