using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using Prism.Mvvm;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings
{
    public class SettingsViewModel : BindableBase
    {
        private readonly IGpoSettings _gpoSettings;

        public SettingsViewModel(IGpoSettings gpoSettings)
        {
            _gpoSettings = gpoSettings;
        }

        public bool ApplicationSettingsIsDisabled
        {
            get { return _gpoSettings != null && _gpoSettings.DisableApplicationSettings; }
        }
    }
}
