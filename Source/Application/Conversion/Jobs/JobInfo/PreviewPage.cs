using System.Drawing;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    public class PreviewPage
    {
        public int PageNumber { get; set; }
        public string PreviewImagePath { get; set; }

        public PreviewPage(int pageNumber, string previewImagePath)
        {
            PreviewImagePath = previewImagePath;
            PageNumber = pageNumber;
        }
    }
}
