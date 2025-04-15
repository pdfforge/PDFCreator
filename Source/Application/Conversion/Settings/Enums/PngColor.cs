using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum PngColor
    {
        [Translation("16 million colors with transparency (32 Bit)")]
        Color32BitTransp,
        [Translation("16 million colors (24 Bit)")]
        Color24Bit,
        [Translation("256 colors (8 Bit)")]
        Color8Bit,
        [Translation("16 colors (4 Bit)")]
        Color4Bit,
        [Translation("Grayscale (8 Bit)")]
        Gray8Bit,
        [Translation("Black & white (2 Bit)")]
        BlackWhite //2Bit
    }
}