using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class MainShellTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
        public string Home { get; private set; } = "Home";
        public string Profiles { get; private set; } = "Profiles";
        public string Printer { get; private set; } = "Printer";
        public string Accounts { get; private set; } = "Accounts";
        public string Settings { get; private set; } = "Settings";
        public string UpdateToolTip { get; private set; } = "An update is available";

        private string UsageStatsInfo { get; set; } = "Help us improve {0} by sending anonymous application usage statistics.";
        public string ReadMore { get; private set; } = "Read More";
        public string UsageStatsDismiss { get; private set; } = "Dismiss";

        public string RssFeedDisabled { get; private set; } = "RSS news feed disabled.";
        public string RssFeedDisabledDescription { get; private set; } = "You can enable the RSS news feed by activating it in the application settings.";

        public string NoRssFeedAvailable { get; private set; } = "No RSS Feed Available";
        public string UnableToReadRssFeed { get; private set; } = "Unable to load the RSS feed. Please check your internet connection.";

        public string RssFeedNewsTitle { get; private set; } = "News";

        public string FormatUsageStatisticsInfoText(string applicationName)
        {
            return string.Format(UsageStatsInfo, applicationName);
        }

        private string WelcomeTextHeadlineText { get; set; } = "Thank you for installing PDFCreator {0}.";
        public string WhatsNew { get; private set; } = "What's new?";
        public string PrioritySupport { get; private set; } = "Priority Support";

        public string GetWelcomeText(string editionNameWithVersion)
        {
            return string.Format(WelcomeTextHeadlineText, editionNameWithVersion);
        }
        
        protected string[] TrialExpiringInfo { get; private set; } = { "Your trial license will expire in {0} day.", "Your trial license will expire in {0} days." };

        protected string TrialExpiredInfo { get; private set; } = "Your trial license expires today.";

        public string LicenseExtendInfo { get; private set; } = "Click here to extend your license.";

        public string GetTrialRemainingDaysInfoText(int trialRemainingDays)
        {
            return trialRemainingDays == 0 ? TrialExpiredInfo : PluralBuilder.GetFormattedPlural(trialRemainingDays, TrialExpiringInfo);
        }
    }
}
