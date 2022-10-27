using Newtonsoft.Json;
using pdfforge.PDFCreator.Core.Services.Update;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
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
