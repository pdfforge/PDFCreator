using System.Collections.Generic;

namespace pdfforge.PDFCreator.Utilities.Update
{
    public interface IUpdateChangeParser
    {
        List<ReleaseInfo> Parse(string json);
    }
}
