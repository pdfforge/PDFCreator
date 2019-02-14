using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Utilities.Tokens;
using pdfforge.PsParser;

namespace pdfforge.PDFCreator.Conversion.Jobs.Jobs
{
    public class UserTokenExtractor : IUserTokenExtractor
    {
        private readonly IPsParserFactory _psParserFactory;

        public UserTokenExtractor(IPsParserFactory psParserFactory)
        {
            _psParserFactory = psParserFactory;
        }

        public UserToken ExtractUserTokenFromPsFile(string psFile, UserTokenSeperator seperator)
        {
            var psParser = BuildPsParser(psFile, seperator);
            psParser.Analyse();
            psParser.RemoveParameterLinesFromPSFile();
            psParser.CloseStream();

            var userToken = new UserToken();
            if (psParser.UserTokens == null)
                return userToken;

            foreach (var ut in psParser.UserTokens)
                userToken.AddKeyValuePair(ut.Key, ut.Value);

            return userToken;
        }

        private IPsParser BuildPsParser(string psfile, UserTokenSeperator seperator)
        {
            string parameterOpenSequence;
            string parameterCloseSequence;

            switch (seperator)
            {
                case UserTokenSeperator.AngleBrackets:
                    parameterOpenSequence = "<<<";
                    parameterCloseSequence = ">>>";
                    break;

                case UserTokenSeperator.CurlyBrackets:
                    parameterOpenSequence = "{{{";
                    parameterCloseSequence = "}}}";
                    break;

                case UserTokenSeperator.RoundBrackets:
                    parameterOpenSequence = "(((";
                    parameterCloseSequence = ")))";
                    break;

                case UserTokenSeperator.SquareBrackets:
                default:
                    parameterOpenSequence = "[[[";
                    parameterCloseSequence = "]]]";
                    break;
            }

            return _psParserFactory.BuildPsParser(psfile, parameterOpenSequence, parameterCloseSequence);
        }
    }
}
