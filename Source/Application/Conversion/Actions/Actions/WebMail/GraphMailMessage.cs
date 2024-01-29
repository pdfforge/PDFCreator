using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.WebMail
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class GraphMailMessage
    {
        public string Subject { get; set; }
        public string Importance { get; set; } = "Normal";
        public bool HasAttachments { get; set; } = true;
        public GraphMailBody Body { get; set; }
        public GraphMailRecipient[] ToRecipients { get; set; }
        public GraphMailRecipient[] CcRecipients { get; set; }
        public GraphMailRecipient[] BccRecipients { get; set; }
    }
}
