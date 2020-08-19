using System.Collections.Generic;
using System.Globalization;

namespace pdfforge.PDFCreator.Core.Services.Translation
{
    public class LanguageProviderEnglishOnly : ILanguageProvider
    {
        private static readonly Language EnglishLanguage = new Language
        {
            CommonName = "English",
            NativeName = "English",
            Iso2 = "en",
            CultureInfo = new CultureInfo("en")
        };

        public Language CurrentLanguage => EnglishLanguage;

        public IEnumerable<Language> GetAvailableLanguages()
        {
            return new[] { EnglishLanguage };
        }

        public bool HasTranslation(string language)
        {
            return language == EnglishLanguage.Iso2;
        }

        public Language FindBestLanguage(CultureInfo culture)
        {
            return EnglishLanguage;
        }

        public Language FindBestLanguage(string language)
        {
            return EnglishLanguage;
        }
    }
}
