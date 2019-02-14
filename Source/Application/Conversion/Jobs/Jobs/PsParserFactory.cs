using pdfforge.PsParser;

namespace pdfforge.PDFCreator.Conversion.Jobs.Jobs
{
    public interface IPsParserFactory
    {
        IPsParser BuildPsParser(string psFile, string parameterOpenSequence, string parameterCloseSequence);
    }

    public class PsParserFactory : IPsParserFactory
    {
        public IPsParser BuildPsParser(string psFile, string parameterOpenSequence, string parameterCloseSequence)
        {
            return new PsParser.PsParser(psFile, parameterOpenSequence, parameterCloseSequence);
        }
    }
}
