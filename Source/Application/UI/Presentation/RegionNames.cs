using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class RegionNames : RegionNameCollection
    {
        public static string MainRegion => NameOfProperty();
        public static string ApplicationSettingsTabsRegion => NameOfProperty();
        public static string OutputFormatOverlayContentRegion => NameOfProperty();
        public static string GeneralSettingsTabContentRegion => NameOfProperty();
        public static string DebugSettingsTabContentRegion => NameOfProperty();
        public static string ApplicationSaveCancelButtonsRegion => NameOfProperty();
        public static string ProfileSaveCancelButtonsRegion => NameOfProperty();
        public static string GeneralSettingsButtonRegion => NameOfProperty();
        public static string GeneralSettingsRegion => NameOfProperty();
        public static string BusinessHintStatusBarRegion => NameOfProperty();
        public static string TestButtonWorkflowEditorRegion => NameOfProperty();
        public static string RssFeedRegion => NameOfProperty();
        public static string HomeViewBannerRegion => NameOfProperty();
        public static string ProfileWorkflowEditorOverlayRegion => NameOfProperty();
        public static string SaveOutputFormatMetadataView => nameof(UserControls.Profiles.SaveOutputFormatMetadataView);
        public static string WorkflowEditorView => nameof(UserControls.Profiles.WorkflowEditor.WorkflowEditorView);
        public static string DirectConversionTabContentRegion => NameOfProperty();
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
