using pdfforge.PDFCreator.Conversion.Settings;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.ActionsInterface
{
    public class CurrentCheckSettings
    {
        public CurrentCheckSettings(IList<ConversionProfile> profiles, IList<PrinterMapping> printerMappings, Accounts accounts)
        {
            Profiles = profiles;
            PrinterMappings = printerMappings;
            Accounts = accounts;
        }

        public IList<ConversionProfile> Profiles { get; }
        public IList<PrinterMapping> PrinterMappings { get; }
        public Accounts Accounts { get; }

        public static CurrentCheckSettings Empty()
        {
            return new CurrentCheckSettings(new List<ConversionProfile>(), new List<PrinterMapping>(), new Accounts());
        }
    }
}
