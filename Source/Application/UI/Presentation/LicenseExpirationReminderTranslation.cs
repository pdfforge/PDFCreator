using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class LicenseExpirationReminderTranslation : ITranslatable
    {
        public string RemindMeLater { get; private set; } = "Remind me later";
        public string ManageLicense { get; private set; } = "Manage license";

        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        private string[] LicenseReminderInfo { get; set; } = { "Your license will expire in {0} day.", "Your license will expire in {0} days." };

        public string FormatLicenseExpiryDate(int remainingDays)
        {
            return PluralBuilder.GetFormattedPlural(remainingDays, LicenseReminderInfo);
        }
    }
}
