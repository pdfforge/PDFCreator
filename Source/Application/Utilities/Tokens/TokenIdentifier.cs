using System.Text.RegularExpressions;

namespace pdfforge.PDFCreator.Utilities.Tokens
{
    public static class TokenIdentifier
    {
        public static bool ContainsTokens(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return false;

            return Regex.IsMatch(parameter, @"<.+?>");
        }

        public static bool ContainsUserToken(string parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter))
                return false;

            return Regex.IsMatch(parameter, @"<User:.*>", RegexOptions.IgnoreCase);
        }
    }
}
