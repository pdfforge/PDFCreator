using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.WebMail
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CreateUploadSessionResponse
    {
        public string UploadUrl { get; set; }
        public string ExpirationDateTime { get; set; }
    }
}
