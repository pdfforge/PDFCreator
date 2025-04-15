using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum TiffColor
    {
        [Translation("16 million colors (24 Bit)")]
        Color24Bit,
        [Translation("4096 colors (12 Bit)")]
        Color12Bit,
        [Translation("Grayscale (8 Bit)")]
        Gray8Bit,
        [Translation("Black & white (2 Bit G3 Fax)")]
        BlackWhiteG3Fax,
        [Translation("Black & white (2 Bit G4 Fax)")]
        BlackWhiteG4Fax,
        [Translation("Black & white (2 Bit LZW)")]
        BlackWhiteLzw
    }
 }
