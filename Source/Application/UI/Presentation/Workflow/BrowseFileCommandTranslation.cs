using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public class BrowseFileCommandTranslation : ITranslatable
    {
        public string PathTooLongTitle { get; private set; } = "The selected path is too long";
        public string PathTooLongText { get; private set; } = "The selected path is too long. Please select a valid path.";
    }
}
