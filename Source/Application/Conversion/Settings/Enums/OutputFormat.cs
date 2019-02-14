using System;
using System.ComponentModel;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    public enum OutputFormat 
    {
        [Description("PDF")] Pdf,
        [Description("PDF/A-1b")] PdfA1B,
        [Description("PDF/A-2b")] PdfA2B,
        [Description("PDF/A-3b")] PdfA3B,
        [Description("PDF/X")] PdfX,
        [Description("JPEG")] Jpeg,
        [Description("PNG")] Png,
        [Description("TIFF")] Tif,
        [Description("Text")] Txt
    }

    public static class OutputFormatExtensions
    {
        public static bool IsPdf(this OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.Pdf:
                case OutputFormat.PdfA1B:
                case OutputFormat.PdfA2B:
                case OutputFormat.PdfA3B:
                case OutputFormat.PdfX:
                    return true;
                case OutputFormat.Jpeg:
                case OutputFormat.Png:
                case OutputFormat.Tif:
                case OutputFormat.Txt:
                    return false;
                default:
                    throw new NotImplementedException($"OutputFormat '{format}' is not known to {nameof(IsPdf)}!");

            }
        }

        public static bool IsPdfA(this OutputFormat format)
        {
            switch (format)
            {
                case OutputFormat.PdfA1B:
                case OutputFormat.PdfA2B:
                case OutputFormat.PdfA3B:
                    return true;
                case OutputFormat.Pdf:
                case OutputFormat.PdfX:
                case OutputFormat.Jpeg:
                case OutputFormat.Png:
                case OutputFormat.Tif:
                case OutputFormat.Txt:
                    return false;
                default:
                    throw new NotImplementedException($"OutputFormat '{format}' is not known to {nameof(IsPdfA)}!");
            }
        }
    }
}