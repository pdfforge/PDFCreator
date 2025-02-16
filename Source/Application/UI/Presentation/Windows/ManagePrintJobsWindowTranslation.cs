using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class ManagePrintJobsWindowTranslation : ITranslatable
    {

        public string AddAnotherFile { get; private set; } = "Add another file";
        public string ContinueButtonContent { get; private set; } = "Continue";
        public string DeleteTooltip { get; private set; } = "Delete print job";
        public string MergeButtonContent { get; private set; } = "Merge";
        public string WindowHeader { get; private set; } = "Reorder and Merge Print Jobs";
        public string DocumentInfoHeader { get; private set; } = "Document Info";
        public string MergedFilesHeader { get; private set; } = "Merged Files";
        public string PrintJobPages { get; private set; } = "Pages:";
        public string PrintJobFiles { get; private set; } = "Files:";
        public string PrintJobTitle { get; private set; } = "Title:";
        public string PrintJobAuthor { get; private set; } = "Author:";
        public string PrintJobPrinter { get; private set; } = "Printer:";
        public string PrintJobCounter { get; private set; } = "ID:";
        public string DragHint { get; private set; } = "Hint: You can drag the print jobs to reorder them";
        public string IdAscending { get; set; } = "Sort by ID ascending";
        public string IdDescending { get; set; } = "Sort by ID descending";

        public string NameAscending { get; set; } = "Sort by name ascending";
        public string NameDescending { get; set; } = "Sort by name descending";

        public string DateAscending { get; set; } = "Sort by date ascending";
        public string DateDescending { get; set; } = "Sort by date descending";
        public string SelectAll { get; set; } = "Select all"; 
        public string Sort { get; set; } = "Sort";
    }
}
