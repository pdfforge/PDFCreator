using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities.Tokens;

namespace pdfforge.PDFCreator.Conversion.Jobs.Jobs
{
    public interface IUserTokenExtractor
    {
        UserToken ExtractUserTokenFromPsFile(string psFile, UserTokenSeperator seperator);
    }
}
