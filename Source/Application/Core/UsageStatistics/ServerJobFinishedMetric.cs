using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class ServerJobFinishedMetric : UsageMetricBase
    {
        public override string EventName => "JobMetric";

        public string OutputFormat { get; set; }
        public string Status { get; set; }
        public long Duration { get; set; }
        public int TotalPages { get; set; } = 0;
        public int NumberOfCopies { get; set; }

        public bool Dropbox { get; set; }
        public bool Ftp { get; set; }
        public bool Smtp { get; set; }
        public bool Http { get; set; }
        public bool Print { get; set; }

        public bool Cover { get; set; }
        public bool Background { get; set; }
        public bool Attachment { get; set; }
        public bool Stamp { get; set; }

        public bool Encryption { get; set; }
        public bool Signature { get; set; }
        public bool DisplaySignatureInDocument { get; set; }

        public bool Script { get; set; }
        public bool CustomScript { get; set; }
        public bool UserToken { get; set; }
    }
}
