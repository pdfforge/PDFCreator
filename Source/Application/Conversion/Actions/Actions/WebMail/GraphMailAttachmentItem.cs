using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.WebMail
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class GraphMailAttachmentItem
    {
        public string AttachmentType { get; set; } = "file";
        public string Name { get; set; }
        public long Size { get; set; }
        public string ContentId { get; set; } = "made_by_PDFCreator";
    }
}
