using System;
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

        public static bool ContainsAnyToken(string input, params string[] tokenNames)
        {
            foreach (var token in tokenNames)
            {
                if (input.IndexOf("<" + token + ">", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return true;
            }
            return false;
        }
    }
}
