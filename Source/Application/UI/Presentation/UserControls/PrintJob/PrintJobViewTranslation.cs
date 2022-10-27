using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PrintJobViewTranslation : ITranslatable
    {
        public string AuthorLabel { get; private set; } = "_Author:";

        [Context("PrintJobWindowButton")]
        public string Cancel { get; private set; } = "_Cancel";

        [Context("PrintJobWindowButton")]
        public string CancelAll { get; private set; } = "Cancel all";

        public string EditProfile { get; private set; } = "Edit";

        public string KeywordsLabel { get; private set; } = "_Keywords:";

        [Context("PrintJobWindowButton")]
        public string Merge { get; private set; } = "_Merge";

        [Context("PrintJobWindowButton")]
        public string MergeAll { get; private set; } = "Merge all";

        public string ProfileLabel { get; private set; } = "_Profile:";

        [Context("PrintJobWindowButton")]
        public string Save { get; private set; } = "_Save";

        public string SaveAs { get; private set; } = "Save as ...";
        public string SaveToDesktop { get; private set; } = "Save to Desktop";

        [Context("PrintJobWindowButton")]
        public string Email { get; private set; } = "_E-mail";

        public string SubjectLabel { get; private set; } = "S_ubject:";
        public string TitleLabel { get; private set; } = "_Title:";
        public string FilenameText { get; private set; } = "File_name:";
        public string DirectoryLabel { get; private set; } = "_Directory:";
        public string SaveTempOnlyIsEnabled { get; private set; } = "You have selected to save files only temporarily. If you want to set the target directory click here.";
        public string RemoveAds { get; private set; } = "Remove ads";
    }
}
