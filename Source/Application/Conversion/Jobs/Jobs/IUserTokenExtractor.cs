using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Conversion.Jobs.Jobs
{
    public interface IUserTokenExtractor
    {
        ParsedFile ParsePdfFileForUserTokens(string pdfFile, UserTokenSeparator separator);
    }

    public class ParsedFile
    {
        public string Filename { get; }
        public UserToken UserToken { get; }
        public int NumberOfPages { get; set; }
        public string SplitDocument { get; set; }

        public ParsedFile(string filename)
        {
            Filename = filename;
            UserToken = new UserToken();
            SplitDocument = null;
            NumberOfPages = 0;
        }

        public ParsedFile(string filename, UserToken userToken, string splitDocument, int numberOfPages)
        {
            Filename = filename;
            UserToken = userToken;
            SplitDocument = splitDocument;
            NumberOfPages = numberOfPages;
        }
    }
}
