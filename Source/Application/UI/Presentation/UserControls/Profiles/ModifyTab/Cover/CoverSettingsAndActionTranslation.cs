using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab.Cover
{
    public class CoverSettingsTranslation : ITranslatable
    {
        public string AllFiles { get; private set; } = "All files";
        public string DisplayName { get; private set; } = "Add cover";
        public string PDFFiles { get; private set; } = "PDF files";
        public string SelectCoverFile { get; private set; } = "Select cover file";
        public string CoverFileText { get; private set; } = "Cover files (PDF):";
        public string WarningIsPdf20 { get; private set; } = "Warning: The following documents are PDF 2.0 files and can't be added to other documents during the conversion:";
    }
}
