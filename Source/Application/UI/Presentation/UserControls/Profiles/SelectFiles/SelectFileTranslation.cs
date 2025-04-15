using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles
{
    public class SelectFileTranslation : ITranslatable
    {
        public string AddFile { get; private set; } = "Add file";
        public string SelectFile { get; private set; } = "Select file:";
        public string AllFiles { get; private set; } = "All files";
        public string Apply { get; private set; } = "Apply";
        public string Remove { get; private set; } = "Remove";
        public string Cancel { get; private set; } = "Cancel";
    }
}
