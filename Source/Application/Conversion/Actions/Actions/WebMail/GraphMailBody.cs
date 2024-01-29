using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.WebMail
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class GraphMailBody
    {
        public string ContentType { get; set; } = "HTML";
        public string Content { get; set; }
    }
}
