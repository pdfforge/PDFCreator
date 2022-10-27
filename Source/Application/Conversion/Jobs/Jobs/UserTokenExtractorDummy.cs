using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Jobs.Jobs
{
    public class UserTokenExtractorDummy : IUserTokenExtractor
    {
        public ParsedFile ParsePdfFileForUserTokens(string pdfFile, UserTokenSeparator separator)
        {
            return new ParsedFile(pdfFile);
        }
    }
}
