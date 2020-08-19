using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.Core.GpoAdapter
{
    public class GpoAwareSettingsProvider : SettingsProvider
    {
        private readonly IGpoSettings _gpoSettings;

        public GpoAwareSettingsProvider(IGpoSettings gpoSettings)
        {
            _gpoSettings = gpoSettings;
        }

        public override string GetApplicationLanguage()
        {
            if (!string.IsNullOrWhiteSpace(_gpoSettings.Language))
            {
                return _gpoSettings.Language;
            }

            return CurrentLanguage;
        }
    }
}
