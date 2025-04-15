using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public class PreviewPages
    {
        public string Directory { get; }
        public IList<PreviewPage> PreviewPageList { get; set; } = new List<PreviewPage>();

        public PreviewPages(string directory)
        {
            Directory = directory;
        }
    }
}
