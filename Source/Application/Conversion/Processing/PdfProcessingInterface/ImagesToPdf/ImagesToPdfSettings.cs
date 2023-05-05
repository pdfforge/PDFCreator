using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface.ImagesToPdf
{
    public class ImagesToPdfSettings
    {
        public Orientation Orientation { get; set; }
        public PdfSize PageSize { get; set; }
        public double Margin { get; set; }
        public MarginType MarginType { get; set; }
        public IDictionary<string, string> Rotation { get; set; }
    }

    public enum Orientation
    {
        Auto,
        Landscape,
        Portrait
    }

    public enum MarginType
    {
        Percent,
        Cm,
        Inch
    }

    public enum PdfSize
    {
        Auto,
        A4,
        Letter,
        Legal
    }
}
