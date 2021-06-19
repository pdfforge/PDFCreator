using System;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.Core.Controller;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.General
{
    public class GeneralSettingsTranslation : ITranslatable
    {
        public string Ask { get; private set; } = "Ask";
        public string AddMenuIntegrationText { get; private set; } = "Add";
        public string AddToContextMenuText { get; private set; } = "Added to context menu";
        public string ChangeDefaultPrinterLabelText { get; private set; } = "Allow PDFCreator to temporarily change the default printer";
        public string CheckUpdateButtonContent { get; private set; } = "Check now";
        public string DefaultPrinterControlHeader { get; private set; } = "Default Printer";
        public string DownloadLatestVersionText { get; private set; } = "Download the latest version at:";
        public string LanguageControlHeader { get; private set; } = "Language";
        public string MenuIntegrationControlHeader { get; private set; } = "Explorer Context Menu Integration";
        public string NeverUpdateWarningText { get; private set; } = "Please note that you won't receive any notifications about available updates!";
        public string RemoveFromContextMenuText { get; private set; } = "Removed from context menu (Maybe you need to restart your computer)";
        public string RemoveMenuIntegrationText { get; private set; } = "Remove";
        public string SelectLanguageLabelText { get; private set; } = "Select the application language:";
        public string TranslatorHintText { get; private set; } = "PDFCreator is translated by volunteers of our translator community. If you would like to contribute your translation skills, please visit our translation page:";
        public string UpdateCheckControlHeader { get; private set; } = "Update";
        public string UpdateCheckIsRunning { get; private set; } = "Update check is already running.";
        public string ErrorMessage { get; private set; } =  "{0} was not able to look for an update.\nPlease check your internet connection and retry later.";
        public string UpdateCheckTitle { get; private set; } = "Update check";
        public string NoUpdateMessage { get; private set; } = "You already have the most recent version.";
        public string UpdateIntervalLabelText { get; private set; } = "Check for updates:";
        public string Yes { get; private set; } = "Yes";
        public string GeneralTabTitle { get; private set; } = "General";

        public string AskLater { get; private set; } = "Ask Later";
        public string SkipVersion { get; private set; } = "Skip version";
        public string Install { get; private set; } = "Install Update";

        public string UsageStatisticsHeader { get; private set; } = "Usage Statistics";
        public string EnableUsageStatistics { get; private set; } = "Send Usage Statistics";
        public string ShowSampleStatistics { get; private set; } = "Show sample statistics";
        public string JobStatisticsExample { get; private set; } = "Job statistics example:";
        public string ServiceStatisticsExample { get; private set; } = "Service statistics example:";
        public string PrivacyPolicy { get; private set; } = "Read our privacy policy here:";
        public string PrivacyPolicyLink { get; private set; } = "Privacy policy";

        public string ConfigureHomeScreen { get; private set; } = "Configure Home Screen";
        public string EnableRssFeed { get; private set; } = "Activate RSS news feed";
        public string EnableTips { get; private set; } = "Show tips";

        private string UsageStatisticsExplanationText { get;  set; } = "Help us improve {0} " +
                                                                             "by sending anonymous application usage statistics. \n" +
                                                                             "This data will only be used by us for analysis purposes not be shared with any third party. \n" +
                                                                             "The application's performance will not be affected by collecting the usage statistics. \n" +
                                                                             "Expand the sample data below to see what data are sent.";
      
        public string UsageStatisticsManualLinkText { get; private set; } = "If you want to know more you can take a look at our manual:";
        public string UsageStatisticsLink { get; private set; } = "Usage statistics";

        public EnumTranslation<UpdateInterval>[] UpdateIntervals { get; private set; } = EnumTranslation<UpdateInterval>.CreateDefaultEnumTranslation();

        private string NewUpdateMessage { get; set; } = "The new version {0} is available.\nWould you like to download the new version?";

        public string GetNewUpdateMessage(string interactionAvailableVersion)
        {
            return string.Format(NewUpdateMessage, interactionAvailableVersion);
        }

        public string GetFormattedErrorMessageWithEditionName(string editionName)
        {
            return string.Format(ErrorMessage, editionName);
        }

        public string FormatUsageStatisticsExplanationText(string applicationName)
        {
            
            return string.Format(UsageStatisticsExplanationText, applicationName);
        }

    }
}
