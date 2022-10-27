using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions.Interface
{
    public interface IOpenFileAction : IPostConversionAction
    {
        ActionResult OpenWithArchitect(List<string> files);

        ActionResult OpenOutputFile(string filePath, bool openWithPdfArchitect = false);
    }
}
