using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Microsoft
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CreateUploadSessionResponse
    {
        public string UploadUrl { get; set; }
        public string ExpirationDateTime { get; set; }
    }
}
