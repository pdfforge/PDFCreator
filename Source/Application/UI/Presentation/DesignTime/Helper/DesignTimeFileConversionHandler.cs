using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeFileConversionAssistant : IFileConversionAssistant
    {
        public void HandleFileList(IEnumerable<string> droppedFiles, AppStartParameters appStartParameters)
        {
        }

        public void HandleFileListWithoutTooManyFilesWarning(IEnumerable<string> droppedFiles, AppStartParameters appStartParameters)
        {
        }
    }
}
