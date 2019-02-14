using pdfforge.PDFCreator.Conversion.Settings.GroupPolicies;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class HomeViewSettingsViewModel : AGeneralSettingsItemControlModel
    {        
        private readonly ICurrentSettings<RssFeed> _rssFeedProvider;
        private readonly ICurrentSettings<ApplicationSettings> _applicationSettingsProvider;

        public bool RssFeedEnabled
        {
            get
            {
                return _rssFeedProvider.Settings.Enable && RssFeedEnabledByGpo;
            }
            set
            {
                _rssFeedProvider.Settings.Enable = value;
                RaisePropertyChanged(nameof(RssFeedEnabled));
            }
        }

        public bool TipsEnabled
        {
            get
            {
                return _applicationSettingsProvider.Settings.EnableTips && TipsEnabledByGpo;
            }
            set
            {
                _applicationSettingsProvider.Settings.EnableTips = value;
                RaisePropertyChanged(nameof(TipsEnabled));
            }
        }

        public bool TipsEnabledByGpo => !GpoSettings.DisableTips;
        public bool RssFeedEnabledByGpo => !GpoSettings.DisableRssFeed;

        public bool ShowHomeConfigurationByGpo => TipsEnabledByGpo || RssFeedEnabledByGpo;

        public HomeViewSettingsViewModel(ITranslationUpdater translationUpdater, ICurrentSettingsProvider settingsProvider, 
                                        IGpoSettings gpoSettings, ICurrentSettings<RssFeed> rssFeedProvider, ICurrentSettings<ApplicationSettings> applicationsSettingsProvider) 
                                        : base(translationUpdater, settingsProvider, gpoSettings)
        {
            _rssFeedProvider = rssFeedProvider;
            _applicationSettingsProvider = applicationsSettingsProvider;
        }
    }
}
