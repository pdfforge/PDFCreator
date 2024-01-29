using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.WebMail
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CreateUploadSessionRequest
    {
        public GraphMailAttachmentItem AttachmentItem { get; set; }
    }
}
