using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class RegionNames : RegionNameCollection
    {
        public static string MainRegion => NameOfProperty();
        public static string ProfileTabContentRegion => NameOfProperty();
        public static string ApplicationSettingsTabsRegion => NameOfProperty();
        public static string SaveTabContentRegion => NameOfProperty();
        public static string ConvertTabContentRegion => NameOfProperty();
        public static string ConvertTabOverlayContentRegion => NameOfProperty();
        public static string GeneralSettingsTabContentRegion => NameOfProperty();
        public static string DebugSettingsTabContentRegion => NameOfProperty();

        public static string ApplicationSaveCancelButtonsRegion => NameOfProperty();
        public static string ProfileSaveCancelButtonsRegion => NameOfProperty();

        public static string ModifyMasterTabItemsRegion => NameOfProperty();
        public static string ModifyMasterTabContentRegion => NameOfProperty();

        public static string SecureMasterTabItemsRegion => NameOfProperty();
        public static string SecureMasterTabContentRegion => NameOfProperty();

        public static string AdvancedMasterTabItemsRegion => NameOfProperty();
        public static string AdvancedMasterTabContentRegion => NameOfProperty();
        public static string WorkflowEditorSideBarRegion => NameOfProperty();

        public static string SendMasterTabItemsRegion => NameOfProperty();
        public static string SendMasterTabContentRegion => NameOfProperty();

        public static string MetadataTabContentRegion => NameOfProperty();
        public static string TitleReplacementTabContentRegion => NameOfProperty();
        public static string DefaultViewerTabContentRegion => NameOfProperty();

        public static string LicenseSettingsRegion => NameOfProperty();

        public static string BusinessHintStatusBarRegion => NameOfProperty();
        public static string TestButtonWorkflowEditorRegion => NameOfProperty();
        public static string AddActionWorkflowEditorRegion => NameOfProperty();

        public static string RssFeedRegion => NameOfProperty();

        public static string HomeViewBannerRegion => NameOfProperty();

        public static string ProfileWorkflowEditorOverlayRegion => NameOfProperty();

        public static string ProfileLayoutRegion => NameOfProperty();

        public static string AboutView => nameof(UserControls.AboutView);
        public static string AccountsView => nameof(UserControls.Accounts.AccountsView);
        public static string ArchitectView => nameof(UserControls.Architect.ArchitectView);
        public static string HomeView => nameof(UserControls.Home.HomeView);
        public static string PrinterView => nameof(UserControls.Printer.PrinterView);
        public static string ProfilesView => nameof(UserControls.Profiles.ProfilesView);
        public static string ApplicationSettingsView => nameof(UserControls.Settings.ApplicationSettingsView);
    }

    public class PrintJobRegionNames : RegionNameCollection
    {
        public static string PrintJobMainRegion => NameOfProperty();
        public static string PrintJobViewBannerRegion => NameOfProperty();
    }

    public class RegionNameCollection
    {
        /// <summary>
        /// Extract values from all string fields
        /// </summary>
        /// <returns>All values of of all string fields, which results in a list of all region names for this class</returns>
        public IEnumerable<string> GetRegionNames()
        {
            var props = GetType().GetProperties();
            foreach (var propertyInfo in props)
            {
                var value = propertyInfo.GetValue(null) as string;
                if (value != null)
                    yield return value;
            }

            var fields = GetType().GetFields();
            foreach (var fieldInfo in fields)
            {
                var value = fieldInfo.GetValue(null) as string;
                if (value != null)
                    yield return value;
            }
        }

        protected static string NameOfProperty([CallerMemberName] string callerName = null)
        {
            return callerName;
        }
    }
}
