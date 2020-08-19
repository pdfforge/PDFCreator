using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.DesignTimeViewModels.Helper
{
    public class DesignTimeFileConversionAssistant : IFileConversionAssistant
    {
        public void HandleFileList(IEnumerable<(string path, AppStartParameters paramters)> droppedFiles)
        {
        }

        public void HandleFileList(IEnumerable<string> droppedFiles)
        {
        }
    }

    public class DesignTimeDragAndDropHandler : DragAndDropEventHandler
    {
        public DesignTimeDragAndDropHandler() : base(new DesignTimeFileConversionAssistant())
        {
        }
    }
}
