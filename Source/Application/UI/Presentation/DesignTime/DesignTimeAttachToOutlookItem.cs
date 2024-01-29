using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Actions.AttachToOutlookItem;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeAttachToOutlookItem : IAttachToOutlookItem
    {
        public IList<string> GetOutlookItemCaptions()
        {
            return new List<string> { "Caption1", "Caption2", "Caption3" };
        }

        public AttachToOutlookItemResult ExportToOutlookItem(string itemCaption, IList<string> files)
        {
            throw new System.NotImplementedException();
        }
    }
}
