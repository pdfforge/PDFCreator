using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface.ImagesToPdf
{
    public interface IImagesToPdf
    {
        void ConvertImage2Pdf(IList<string> directConversionFiles, ApplicationSettings appSettings, string outputFile);
    }
}
