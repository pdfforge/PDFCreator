using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using pdfforge.PDFCreator.Conversion.Actions.Actions.WebMail;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Microsoft
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CreateUploadSessionRequest
    {
        public GraphMailAttachmentItem AttachmentItem { get; set; }
    }
}
