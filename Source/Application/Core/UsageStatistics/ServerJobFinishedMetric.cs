using pdfforge.UsageStatistics;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    public class ServerJobFinishedMetric : UsageMetricBase
    {
        public override string EventName => "JobMetric";

        public string OperatingSystem { get; set; }

        //Job
        public string OutputFormat { get; set; }
        public bool SaveFileTemporarily { get; set; }
        public string AutoSaveExistingFileBehaviour { get; set; }
        public bool SkipSendFailures { get; set; }

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
        public bool Smtp { get; set; }
        public bool SmtpSendOnBehalfOf { get; set; }
        public bool SmtpReplyTo { get; set; }
        public bool Script { get; set; }
        public bool Print { get; set; }
        public bool Ftp { get; set; }
        public bool Http { get; set; }
        public bool Dropbox { get; set; }
    }
}
