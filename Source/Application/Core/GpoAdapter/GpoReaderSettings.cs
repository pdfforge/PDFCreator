using pdfforge.GpoReader;
using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;

namespace pdfforge.PDFCreator.Core.GpoAdapter
{
    public class GpoReaderSettings : IGpoSettings
    {
        private readonly GpoSettings _gpoSettings;

        public GpoReaderSettings(GpoSettings gpoSettings)
        {
            _gpoSettings = gpoSettings;
        }

        public bool DisableApplicationSettings => _gpoSettings.DisableApplicationSettings;
        public bool DisableDebugTab => _gpoSettings.DisableDebugTab;
        public bool DisablePrinterTab => _gpoSettings.DisablePrinterTab;
        public bool DisableProfileManagement => _gpoSettings.DisableProfileManagement;
        public bool DisableTitleTab => _gpoSettings.DisableTitleTab;
        public bool DisableAccountsTab => _gpoSettings.DisableAccountsTab;
        public bool DisableUsageStatistics => _gpoSettings.DisableUsageStatistics;
        public bool HideLicenseTab => _gpoSettings.HideLicenseTab;
        public bool HidePdfArchitectInfo => _gpoSettings.HidePdfArchitectInfo;
        public string Language => _gpoSettings.Language;
        public string UpdateInterval => _gpoSettings.UpdateInterval;
        public bool DisableRssFeed => _gpoSettings.DisableRssFeed;
        public bool DisableTips => _gpoSettings.DisableTips;
        public bool LoadSharedAppSettings => _gpoSettings.LoadSharedAppSettings;
        public bool LoadSharedProfiles => _gpoSettings.LoadSharedProfiles;
        public bool AllowUserDefinedProfiles => _gpoSettings.AllowUserDefinedProfiles;
    }
}
