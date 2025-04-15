using System.Threading;
using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class PdfCreatorJobFinishedMetric : UsageMetricBase
    {
        public override string EventName => "PdfCreatorMetric";
        public string OperatingSystem { get; set; }

        //Job
        public string OutputFormat { get; set; }
        public Mode Mode { get; set; }
        public bool SaveFileTemporarily { get; set; }
        public string AutoSaveExistingFileBehaviour { get; set; }
        public bool SkipSendFailures { get; set; }
        public bool ShowTrayNotifications { get; set; }
        public bool QuickActions { get; set; }

        //Result
        public int TotalPages { get; set; } = 0;
        public int NumberOfCopies { get; set; }
        public long Duration { get; set; }
        public string Status { get; set; }
        
        //Preparation Actions
        public bool CustomScript { get; set; }
        public bool UserToken { get; set; }
        public bool ForwardToFurtherProfile { get; set; }

        //Modify Actions
        public bool Cover { get; set; }
        public bool Attachment { get; set; }
        public bool Stamp { get; set; }
        public bool Watermark { get; set; }
        public bool Background { get; set; }
        public bool PageNumbers { get; set; }
        public bool Encryption { get; set; }
        public bool Signature { get; set; }
        public bool DisplaySignatureInDocument { get; set; }

        //Send actions
        public bool Mailclient { get; set; }
        public bool Smtp { get; set; }
        public bool SmtpSendOnBehalfOf { get; set; }
        public bool SmtpReplyTo { get; set; }
        public bool MailWebAccess { get; set; }
        public bool MailWebAccessSendMailAutomatically { get; set; }
        public bool OpenViewer { get; set; }
        public bool OpenWithPdfArchitect { get; set; }
        public bool Script { get; set; }
        public bool Print { get; set; }
        public bool Ftp { get; set; }
        public bool Http { get; set; }
        public bool OneDrive { get; set; }
        public bool OneDriveShareLink { get; set; }
        public bool Dropbox { get; set; }
        public bool Sharepoint { get; set; }
        public bool SharepointShareLink { get; set; }

        //GPOs
        public bool DisableApplicationSettings { get; set; }
        public bool DisableDebugTab { get; set; }
        public bool DisablePrinterTab { get; set; }
        public bool DisableProfileManagement { get; set; }
        public bool DisableTitleTab { get; set; }
        public bool DisableHistory { get; set; }
        public bool DisableAccountsTab { get; set; }
        public bool DisableRssFeed { get; set; }
        public bool DisableTips { get; set; }
        public bool HideLicenseTab { get; set; }
        public bool HidePdfArchitectInfo { get; set; }
        public string GpoLanguage { get; set; }
        public string GpoUpdateInterval { get; set; }
        public bool DisableLicenseExpirationReminder { get; set; }
        public int? GpoHotStandbyMinutes { get; set; }

        //Share settings
        public bool LoadSharedAppSettings { get; set; }
        public bool LoadSharedProfiles { get; set; }
        public bool IsShared { get; set; }
        public bool AllowUserDefinedProfiles { get; set; }
        public bool HasShareFilename { get; set; }

        //Opened Windows
        public bool OpenedMainShell { get; set; }
    }
}
