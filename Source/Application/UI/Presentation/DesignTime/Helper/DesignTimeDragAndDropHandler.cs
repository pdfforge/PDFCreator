using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeDragAndDropHandler : DragAndDropEventHandler
    {
        public DesignTimeDragAndDropHandler() : base(new DesignTimeFileConversionAssistant())
        {
        }
    }
}
