using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class RegionNames : RegionNameCollection
    {
        public static string MainRegion = "MainRegion";
        public static string ProfileTabContentRegion = "ProfileTabRegion";
        public static string SettingsTabRegion = "SettingsTabRegion";
        public static string SaveTabContentRegion = "SaveTabContentRegion";
        public static string ConvertTabContentRegion = "ConvertTabContentRegion";
        public static string GeneralSettingsTabContentRegion = "GeneralSettingsTabContentRegion";
        public static string DebugSettingsTabContentRegion = "DebugSettingsTabContentRegion";
        public static string ApplicationSaveCancelButtonsRegion = "ApplicationSaveCancelButtonsRegion";

        public static string ProfileSaveCancelButtonsRegion = "ProfileSaveCancelButtonsRegion";

        public static string ModifyMasterTabItemsRegion = "ModifyMasterTabItemsRegion";
        public static string ModifyMasterTabContentRegion = "ModifyMasterTabContentRegion";

        public static string SecureMasterTabItemsRegion = "SecureMasterTabItemsRegion";
        public static string SecureMasterTabContentRegion = "SecureMasterTabContentRegion";

        public static string AdvancedMasterTabItemsRegion = "AdvancedMasterTabItemsRegion";
        public static string AdvancedMasterTabContentRegion = "AdvancedMasterTabContentRegion";

        public static string SendMasterTabItemsRegion = "SendMasterTabItemsRegion";
        public static string SendMasterTabContentRegion = "SendMasterTabContentRegion";

        public static string MetadataTabContentRegion = "MetadataTabContent";
        public static string TitleReplacementTabContentRegion = "TitleReplacementTabContentRegion";
        public static string DefaultViewerTabContentRegion = "DefaultViewerTabContentRegion";

        public static string LicenseSettingsContentRegion = "LicenseSettingsContentRegion";

        public static string StatusBarPlusHintRegion = "StatusBarPlusHintRegion";

        public static string UsageStatisticsTabContentRegion = "UsageStatisticsTabContentRegion";

        public static string RssFeedRegion = "RssService";

        public static string HomeViewBannerRegion = "HomeViewBannerRegion";
    }

    public class PrintJobRegionNames : RegionNameCollection
    {
        public static string PrintJobMainRegion = "PrintJobMainRegion";
    }

    public class RegionNameCollection
    {
        /// <summary>
        /// Extract values from all string fields
        /// </summary>
        /// <returns>All values of of all string fields, which results in a list of all region names for this class</returns>
        public IEnumerable<string> GetRegionNames()
        {
            var props = GetType().GetFields();
            foreach (var fieldInfo in props)
            {
                var value = fieldInfo.GetValue(null) as string;
                if (value != null)
                    yield return value;
            }
        }
    }
}
