using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using System.Collections.ObjectModel;

namespace pdfforge.PDFCreator.UI.Presentation.NavigationChecks
{
    public class NavigateProfileCheck : ISettingsNavigationCheck
    {
        private readonly ICurrentSettings<ObservableCollection<ConversionProfile>> _profilesProvider;
        private readonly ICurrentSettings<Accounts> _accountsProvider;
        private readonly IProfileChecker _profileChecker;
        private readonly ISettingsChanged _settingsChanged;

        public string ProfileSettingsRegionName { get; set; } = MainRegionViewNames.ProfilesView;

        public NavigateProfileCheck(ICurrentSettings<ObservableCollection<ConversionProfile>> profilesProvider, ICurrentSettings<Accounts> accountsProvider, IProfileChecker profileChecker, ISettingsChanged settingsChanged)
        {
            _profilesProvider = profilesProvider;
            _accountsProvider = accountsProvider;
            _profileChecker = profileChecker;
            _settingsChanged = settingsChanged;
        }

        public SettingsCheckResult CheckSettings()
        {
            var resultDict = _profileChecker.CheckProfileList(_profilesProvider.Settings, _accountsProvider.Settings);
            var settingsChanged = _settingsChanged.HaveChanged();

            return new SettingsCheckResult(resultDict, settingsChanged);
        }

        public bool IsRelevantForRegion(string region)
        {
            return region == ProfileSettingsRegionName;
        }
    }
}
