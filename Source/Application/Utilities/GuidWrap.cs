using System;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IGuid
    {
        string NewGuidString();
    }

    public class GuidWrap : IGuid
    {
        public string NewGuidString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
