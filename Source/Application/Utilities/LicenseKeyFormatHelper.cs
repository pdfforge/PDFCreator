using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace pdfforge.PDFCreator.Utilities
{
    public enum FormatValidationResult
    {
        Valid,
        InvalidCharacters,
        WrongFormat
    }

    public class LicenseKeyFormatHelper
    {
        public string NormalizeLicenseKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "";

            var normalizedKey = key.Replace("-", "").ToUpper().Trim();
            //The %5 as condition for a trailing dash only works before the join with the "-" 
            var appendDash = normalizedKey.Length > 0 && normalizedKey.Length % 5 == 0;
            normalizedKey = string.Join("-", Split(normalizedKey, 5));
            
            if (appendDash && normalizedKey.Length < 35)
                normalizedKey += "-";
            
            return normalizedKey;
        }

        private IEnumerable<string> Split(string str, int chunkSize)
        {
            var chunks = (int)Math.Ceiling(str.Length / (double)chunkSize);

            return Enumerable.Range(0, chunks)
                .Select(i => GetSafeSubstring(str, i * chunkSize, chunkSize));
        }

        private string GetSafeSubstring(string str, int position, int length)
        {
            if (position + length > str.Length)
                length = str.Length - position;

            return str.Substring(position, length);
        }

        public FormatValidationResult ValidateFormat(string s)
        {
            var normalizedKey = NormalizeLicenseKey(s);

            var invalidCharactersRegEx = new Regex("^[A-Z0-9]*$");
            if (!invalidCharactersRegEx.IsMatch(normalizedKey.Replace("-", "")))
                return FormatValidationResult.InvalidCharacters;

            var wrongFormatRegex = new Regex("[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}-[A-Z0-9]{5}");
            if (!wrongFormatRegex.IsMatch(normalizedKey))
                return FormatValidationResult.WrongFormat;

            return FormatValidationResult.Valid;
        }
    }
}
