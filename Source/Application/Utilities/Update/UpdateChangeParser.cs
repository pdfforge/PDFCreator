using System.Collections.Generic;
using Newtonsoft.Json;

namespace pdfforge.PDFCreator.Utilities.Update
{
    public class UpdateChangeParser : IUpdateChangeParser
    {
        public List<ReleaseInfo> Parse(string json)
        {
            var obj = JsonConvert.DeserializeObject<List<ReleaseInfo>>(json);
            return obj;
        }
    }
}
