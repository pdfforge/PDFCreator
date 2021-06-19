using pdfforge.PDFCreator.Core.Workflow;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.NavigationChecks
{
    public class NavigateProfileCheck : ISettingsNavigationCheck
    {
        private readonly ICurrentSettingsProvider _currentSettingsProvider;
        private readonly IProfileChecker _profileChecker;
        private readonly ISettingsChanged _settingsChanged;

        public string ProfileSettingsRegionName { get; set; } = RegionNames.ProfilesView;

        public NavigateProfileCheck(ICurrentSettingsProvider currentSettingsProvider, IProfileChecker profileChecker, ISettingsChanged settingsChanged)
        {
            _currentSettingsProvider = currentSettingsProvider;
            _profileChecker = profileChecker;
            _settingsChanged = settingsChanged;
        }

        public SettingsCheckResult CheckSettings()
        {
            var settings = _currentSettingsProvider.CheckSettings;
            var resultDict = _profileChecker.CheckProfileList(settings);
            var settingsChanged = _settingsChanged.HaveChanged();

            return new SettingsCheckResult(resultDict, settingsChanged);
        }

        public bool IsRelevantForRegion(string region)
        {
            return region == ProfileSettingsRegionName;
        }
    }
}
