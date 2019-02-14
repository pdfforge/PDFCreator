using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class PrintJobViewTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        public string AuthorLabel { get; private set; } = "_Author:";

        [Context("PrintJobWindowButton")]
        public string CancelButton { get; private set; } = "_Cancel";

        public string ConfirmSaveAs { get; private set; } = "Confirm Save As";
        public string EditProfile { get; private set; } = "Edit";

        private string FileAlreadyExists { get; set; } = "'{0}' already exists.\nDo you want to replace it?";

        public string GetFileAlreadyExists(string filepath)
        {
            return string.Format(FileAlreadyExists, filepath);
        }

        public string KeywordsLabel { get; private set; } = "_Keywords:";

        [Context("PrintJobWindowButton")]
        public string MergeButton { get; private set; } = "_Merge";

        public string ProfileLabel { get; private set; } = "_Profile:";

        [Context("PrintJobWindowButton")]
        public string SaveButton { get; private set; } = "_Save";

        [Context("PrintJobWindowButton")]
        public string EmailButton { get; private set; } = "_E-mail";

        public string SubjectLabel { get; private set; } = "S_ubject:";
        public string TitleLabel { get; private set; } = "_Title:";
        public string FilenameText { get; private set; } = "File_name:";
        public string FoldernameText { get; private set; } = "_Folder:";
        public string PathTooLongTitle { get; private set; } = "The selected path is too long";
        public string PathTooLongText { get; private set; } = "The selected path is too long. Please select a valid path.";
    }
}
