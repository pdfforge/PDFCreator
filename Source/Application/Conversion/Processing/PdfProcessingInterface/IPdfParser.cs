using pdfforge.PDFCreator.Conversion.Jobs.Jobs;

namespace pdfforge.PDFCreator.Conversion.Processing.PdfProcessingInterface
{
    public interface IPdfParser
    {
        ParsedFile ParseDocument(string pdfFile);
    }

    public interface IPdfParserFactory
    {
        IPdfParser BuildPdfParser(string parameterOpenSequence, string parameterCloseSequence);
    }
}
