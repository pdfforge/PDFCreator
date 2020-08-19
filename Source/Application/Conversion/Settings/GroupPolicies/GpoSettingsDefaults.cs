namespace pdfforge.PDFCreator.Conversion.Settings.GroupPolicies
{
    public class GpoSettingsDefaults : IGpoSettings
    {
        public bool DisableApplicationSettings => false;
        public bool DisableDebugTab => false;
        public bool DisablePrinterTab => false;
        public bool DisableProfileManagement => false;
        public bool DisableTitleTab => false;
        public bool DisableHistory => false;
        public bool DisableAccountsTab => false;
        public bool DisableUsageStatistics => false;
        public bool DisableRssFeed => false;
        public bool DisableTips => false;
        public bool HideLicenseTab => false;
        public bool HidePdfArchitectInfo => false;
        
        public string Language => null;
        public string UpdateInterval => null;

        public bool LoadSharedAppSettings => false;
        public bool LoadSharedProfiles => false;
        public bool AllowUserDefinedProfiles => true;

        public bool AllowSharedProfilesEditing => false;
        public bool DisableLicenseExpirationReminder => false;
    }
}