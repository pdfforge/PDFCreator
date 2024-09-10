using System.Collections.Generic;
using Optional;
using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeOpenFileInteractionHelper : IOpenFileInteractionHelper
    {
        public Option<string> StartOpenFileInteraction(string currentPath, string title, string filter)
        {
            return new Option<string>();
        }

        public Option<List<string>> StartOpenMultipleFilesInteraction(string currentPath, string title, string filter)
        {
            return new Option<List<string>>();
        }
    }
}
