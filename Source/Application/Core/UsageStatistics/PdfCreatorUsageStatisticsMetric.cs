namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class PdfCreatorUsageStatisticsMetric : IUsageMetric
    {
        public string EventName => "PdfCreatorMetric";

        public string Product { get; set; }
        public string MachineId { get; set; }
        public string Version { get; set; }
        public string OperatingSystem { get; set; }
        public Mode Mode { get; set; }
        public bool QuickActions { get; set; }
        public bool OpenViewer { get; set; }
        public bool OpenWithPdfArchitect { get; set; }
        public string OutputFormat { get; set; }
        public string Status { get; set; }
        public long Duration { get; set; }
        public int TotalPages { get; set; } = 0;
        public int NumberOfCopies { get; set; }

        public bool Dropbox { get; set; }
        public bool Ftp { get; set; }
        public bool Smtp { get; set; }
        public bool Mailclient { get; set; }
        public bool Http { get; set; }
        public bool Print { get; set; }

        public bool Cover { get; set; }
        public bool Background { get; set; }
        public bool Attachment { get; set; }
        public bool Stamp { get; set; }

        public bool Encryption { get; set; }
        public bool Signature { get; set; }

        public bool Script { get; set; }
        public bool DisplaySignatureInDocument { get; set; }

        public bool CustomScript { get; set; }
        public bool UserToken { get; set; }
        public bool IsShared { get; set; }

        public bool DisableApplicationSettings { get; set; }
        public bool DisableDebugTab { get; set; }
        public bool DisablePrinterTab { get; set; }
        public bool DisableProfileManagement { get; set; }
        public bool DisableTitleTab { get; set; }
        public bool DisableAccountsTab { get; set; }
        public bool DisableRssFeed { get; set; }
        public bool DisableTips { get; set; }
        public bool HideLicenseTab { get; set; }
        public bool HidePdfArchitectInfo { get; set; }
        public string GpoLanguage { get; set; }
        public string GpoUpdateInterval { get; set; }
        public bool LoadSharedAppSettings { get; set; }
        public bool LoadSharedProfiles { get; set; }
        public bool AllowUserDefinedProfiles { get; set; }
    }
}
