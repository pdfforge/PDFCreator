﻿namespace pdfforge.PDFCreator.Utilities
{
    public static class Urls
    {
        public const string ArchitectDownloadUrl = "https://www.pdfarchitect.org/download/?configid=7337901D-7D94-481D-ABFC-4314E1698065";
        public const string ArchitectWebsiteUrl = "https://go.pdfforge.org/pdfarchitect/info";
        public const string SodaPdfWebsiteUrl = "https://go.pdfforge.org/pdfcreator/recommend-soda";
        public const string Facebook = "https://go.pdfforge.org/facebook";

        public const string ProfessionalHintUrl = "https://go.pdfforge.org/pdfcreator-plus/inapp-hint";
        public const string BusinessHintLink = "https://go.pdfforge.org/pdfcreator-business/inapp-hint";

        public const string PdfforgeWebsiteUrl = "https://www.pdfforge.org";
        public const string PdfforgeTranslationUrl = "https://translate.pdfforge.org/projects/pdfcreator/#information";
        public const string PrioritySupport = "https://go.pdfforge.org/priority-support";

        public const string Forums = "https://go.pdfforge.org/forums";
        public const string KnowledgeBase = "https://go.pdfforge.org/knowledgebase";
        public const string Twitter = "https://go.pdfforge.org/twitter";

        public const string PDFCreatorDownloadUrl = "https://go.pdfforge.org/pdfcreator/download";

        public const string OfflineActivationUrl = "https://go.pdfforge.org/pdfcreator/offline-activation";

        public const string PdfCreatorUpdateInfoUrl = "https://update.pdfforge.org/pdfcreator/update-info.txt";
        public const string PdfCreatorUpdateChangelogUrl = "https://update.pdfforge.org/pdfcreator/updates";

        public const string PdfCreatorServerUpdateInfoUrl = "https://update.pdfforge.org/pdfcreator-server/update-info.txt";
        public const string PdfCreatorServerUpdateChangelogUrl = "https://update.pdfforge.org/pdfcreator-server/updates";

        public const string PdfCreatorProfessionalUpdateInfoUrl = "https://update.pdfforge.org/pdfcreator-professional/update-info.txt";
        public const string PdfCreatorProfessionalUpdateChangelogUrl = "https://update.pdfforge.org/pdfcreator-professional/updates";

        public const string PdfCreatorTerminalServerUpdateInfoUrl = "https://update.pdfforge.org/pdfcreator-terminal-server/update-info.txt";
        public const string PdfCreatorTerminalServerUpdateChangelogUrl = "https://update.pdfforge.org/pdfcreator-terminal-server/updates";

        public const string PdfCreatorTerminalServerUrl = "https://go.pdfforge.org/pdfcreator-terminal-server";

        public const string LicenseServerManageLicenses = "https://go.pdfforge.org/pdfcreator/manage-licenses";
        public const string LicenseServerManageSingleLicense = "https://go.pdfforge.org/pdfcreator/manage-single-license";

        public const string UsageStatisticsEndpointUrl = "https://stat.pdfforge.org/event/api/v1/single/";

        //public const string AvqUsageStatisticsEndpointUrl = "https://go.pdfforge.org/pdfcreator/AvqUsageStatistics";
        public const string AvqUsageStatisticsStagingEndpointUrl = "https://stage-inapp.pdfcreator.com/api/v1/event";

        public const string PrivacyPolicyUrl = "https://go.pdfforge.org/privacy-policy";

        public const string RssFeedUrl = "https://www.pdfforge.org/blog/rss";

        public const string PdfCreatorOnlineUrl = "https://go.pdfforge.org/pdfcreator/online";

        public const string SentryDsnUrl = "https://83989e10cffe463194d1a14a7ed97828@sentry.pdfforge.org/25";

        public const string UserGuideCommandLineUrl = "https://docs.pdfforge.org/pdfcreator/en/pdfcreator/using-pdfcreator/command-line-parameters/";

        /*Tips*/
        public const string Tip_AutoSaveUrl = "https://go.pdfforge.org/pdfcreator/tips/auto-save";
        public const string Tip_UserTokensUrl = "https://go.pdfforge.org/pdfcreator/tips/user-tokens";
        public const string Tip_WorkflowUrl = "https://go.pdfforge.org/pdfcreator/tips/workflow";
        public static string Tip_DropBoxUrl = "https://go.pdfforge.org/pdfcreator/tips/send-large-files";

        public const string BannerIndexUrl = "https://go.pdfforge.org/pdfcreator/banners/v1";
        public const string BannerIndexUrlStaging = "https://go.pdfforge.org/pdfcreator/banners-staging/v1";

        /*Extend Trial Links*/
        private const string PdfCreatorProfessionalExtend = "https://go.pdfforge.org/pdfcreator-professional/extend-trial";
        private const string PdfCreatorTerminalServerExtend = "https://go.pdfforge.org/pdfcreator-terminal-server/extend-trial";
        private const string PdfCreatorServerExtend = "https://go.pdfforge.org/pdfcreator-server/extend-trial";
        private const string CustomerPortalManageLicense = "https://go.pdfforge.org/pdfcreator/manage-single-license";

        //AHA! Feedback
        public const string AhaUrlStaging = "https://stage-api-feedback.avanquest.com/api/feedback";
        public const string AhaUrl = "https://api-feedback.avanquest.com/api/feedback";

        public static string GetExtendLicenseFallbackUrl(string edition)
        {
            switch (edition.ToLowerInvariant().Replace(" ", "-").Trim())
            {
                case "professional":
                    return PdfCreatorProfessionalExtend;

                case "terminal-server":
                    return PdfCreatorTerminalServerExtend;

                case "server":
                    return PdfCreatorServerExtend;

                default:
                    return CustomerPortalManageLicense;
            }
        }
    }
}
