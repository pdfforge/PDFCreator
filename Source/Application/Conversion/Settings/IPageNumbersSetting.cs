using System.Drawing;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    interface IPageNumbersSetting
    {
        bool Enabled { get; set; }
        Color FontColor { get; set; }

        string FontName { get; set; }

        string FontFile { get; set; }

        float FontSize { get; set; }

        string Format { get; set; }

        float HorizontalOffset { get; set; }

        PageNumberPosition Position { get; set; }

        bool UseRomanNumerals { get; set; }

        float VerticalOffset { get; set; }

        bool AlternateCorner { get; set; }
    }
}
