using Newtonsoft.Json;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public class UpdateChangeParser : IUpdateChangeParser
    {
        public List<Release> Parse(string json)
        {
            var obj = JsonConvert.DeserializeObject<List<Release>>(json);
            return obj;
        }
    }
}
