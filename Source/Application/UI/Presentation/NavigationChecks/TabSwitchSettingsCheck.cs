using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.NavigationChecks
{
    public interface ITabSwitchSettingsCheck
    {
        SettingsCheckResult CheckAffectedSettings();

        SettingsCheckResult CheckAllSettings();
    }

    public class TabSwitchSettingsCheck : ITabSwitchSettingsCheck
    {
        private readonly IRegionHelper _regionHelper;
        private readonly IEnumerable<ISettingsNavigationCheck> _settingsNavigationChecks;

        public TabSwitchSettingsCheck(IRegionHelper regionHelper, IEnumerable<ISettingsNavigationCheck> settingsNavigationChecks)
        {
            _settingsNavigationChecks = settingsNavigationChecks;
            _regionHelper = regionHelper;
        }

        public SettingsCheckResult CheckAffectedSettings()
        {
            var currentRegion = _regionHelper.CurrentRegionName;
            foreach (var check in _settingsNavigationChecks)
            {
                if (check.IsRelevantForRegion(currentRegion))
                {
                    return check.CheckSettings();
                }
            }
            return new SettingsCheckResult();
        }

        public SettingsCheckResult CheckAllSettings()
        {
            var result = new SettingsCheckResult();
            foreach (var check in _settingsNavigationChecks)
            {
                result.Merge(check.CheckSettings());
            }
            return result;
        }
    }
}
