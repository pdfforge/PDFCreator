using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace pdfforge.PDFCreator.Core.UsageStatistics
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Mode
    {
        Interactive,
        AutoSave,
        Com
    }
}
