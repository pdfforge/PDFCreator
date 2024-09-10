using IGpoSettings = pdfforge.PDFCreator.Conversion.Settings.GroupPolicies.IGpoSettings;

namespace pdfforge.PDFCreator.Core.SettingsManagement.GPO
{
    public class GpoReaderDebugSettings : IGpoSettings
    {
        public bool DisableApplicationSettings => true;
        public bool DisableDebugTab => true;
        public bool DisablePrinterTab => true;
        public bool DisableProfileManagement => true;
        public bool DisableTitleTab => true;
        public bool DisableHistory => true;
        public bool DisableAccountsTab => true;
        public bool DisableUsageStatistics => true;
        public bool HideLicenseTab => true;
        public bool HidePdfArchitectInfo => true;
        public string Language => "en";
        public string UpdateInterval => "Never";
        public string SharedSettingsFilename => "";

        public int? HotStandbyMinutes => 0;

        public bool DisableRssFeed => true;
        public bool DisableTips => true;
        public bool LoadSharedAppSettings => true;
        public bool LoadSharedProfiles => true;
        public bool AllowUserDefinedProfiles => true;
        public bool DisableLicenseExpirationReminder => true;

        public string PageSize => "Automatic";
        public string PageOrientation => "Automatic";

        public bool HideFeedbackForm => true;
    }
}
