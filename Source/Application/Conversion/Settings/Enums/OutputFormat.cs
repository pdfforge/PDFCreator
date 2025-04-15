using System;
using System.ComponentModel;
using Translatable;

namespace pdfforge.PDFCreator.Conversion.Settings.Enums
{
    [Translatable]
    public enum OutputFormat 
    {
        [Translation("PDF")]
        [Description("PDF")] 
        Pdf,
        [Translation("PDF/A-1b")]
        [Description("PDF/A-1b")] 
        PdfA1B,
        [Translation("PDF/A-2b")]
        [Description("PDF/A-2b")] 
        PdfA2B,
        [Translation("PDF/A-3b")]
        [Description("PDF/A-3b")] 
        PdfA3B,
        [Translation("PDF/X")]
        [Description("PDF/X")] 
        PdfX,
        [Translation("JPEG")]
        [Description("JPEG")] 
        Jpeg,
        [Translation("PNG")]
        [Description("PNG")]
        Png,
        [Translation("TIFF")]
        [Description("TIFF")]
        Tif,
        [Translation("Text")]
        [Description("Text")]
        Txt
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

        public static string GetDescription(this OutputFormat format)
        {
            var type = typeof(OutputFormat);
            var memInfo = type.GetMember(format.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            var description = ((DescriptionAttribute) attributes[0]).Description;
            return description;
        }
    }
}