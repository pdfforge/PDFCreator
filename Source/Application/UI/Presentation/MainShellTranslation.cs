using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation
{
    public class MainShellTranslation : ITranslatable
    {
        public string Home { get; private set; } = "Home";
        public string Profiles { get; private set; } = "Profiles";
        public string Printer { get; private set; } = "Printer";
        public string Accounts { get; private set; } = "Accounts";
        public string UpdateToolTip { get; private set; } = "A new Update is available";

        private string UsageStatsInfo { get; set; } = "Help us improve {0} by sending anonymous application usage statistics.";
        public string UsageStatsMore { get; private set; } = "Read More";
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
    }
}
