using System.Globalization;
using NGettext;
using System.IO;

namespace pdfforge.PDFCreator.Core.Services.Translation
{
    public interface IGettextCatalogBuilder
    {
        ICatalog GetCatalog(string messageDomain, string languageName, CultureInfo cultureInfo);
    }

    public class GettextCatalogBuilder : IGettextCatalogBuilder
    {
        private readonly string _localeFolder;

        public GettextCatalogBuilder(string localeFolder)
        {
            _localeFolder = localeFolder;
        }

        public ICatalog GetCatalog(string messageDomain, string languageName, CultureInfo cultureInfo)
        {
            var messageFile = $"{_localeFolder}\\{languageName}\\LC_MESSAGES\\{messageDomain}.mo";
            if (!File.Exists(messageFile))
                return new Catalog(new CultureInfo("en"));

            using (var s = File.OpenRead(messageFile))
            {
                return new Catalog(s, cultureInfo);
            }
        }
    }
}
