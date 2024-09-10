
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Helper.Microsoft
{
    public class FileUploadResult
    {
        public string WebUrl { get; set; }
        public ItemReference ParentReference { get; set; }
    }

    public class ItemReference
    {
        public string DriveId { get; set; }
        public string Id { get; set; }
    }
}
