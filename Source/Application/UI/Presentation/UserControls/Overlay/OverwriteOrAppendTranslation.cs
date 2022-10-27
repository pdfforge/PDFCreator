using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Overlay
{
    public class OverwriteOrAppendTranslation : ITranslatable
    {
        public string OverwriteOrAppendWarningText { get; private set; } = "File with given file name already exist. You can overwrite the old file with the new file or merge the new pages into the existing file.";
        public string OverwriteWarningText { get; private set; } = "File with given file name already exist. You can overwrite the old file or cancel to select a different file.";
        public string Overwrite { get; private set; } = "_Overwrite";
        public string Merge { get; private set; } = "_Merge";
        public string Cancel { get; private set; } = "_Cancel";
        public string Title { get; private set; } = "File already exists";
    }
}
