namespace pdfforge.PDFCreator.UI.Presentation.NavigationChecks
{
    /// <summary>
    /// The ISettingsNavigationChecks are called when navigation away from certain regions
    /// </summary>
    public interface ISettingsNavigationCheck
    {
        /// <summary>
        /// Determines if the Check is relevant for navigation away from the given region name
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        bool IsRelevantForRegion(string region);

        /// <summary>
        /// Perform a check on implementation-dependent settings during navigation
        /// </summary>
        /// <returns>A SettingsCheckResult which determines if the settings were changed and if errors exist</returns>
        SettingsCheckResult CheckSettings();
    }
}
