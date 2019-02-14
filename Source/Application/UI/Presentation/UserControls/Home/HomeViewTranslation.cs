using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Home
{
    public class HomeViewTranslation : ITranslatable
    {
        private string CallToAction { get; set; } = "Print to '{0}' to get started!";
        public string ChooseFileButton { get; private set; } = "Choose a file to convert";
        public string HistoryIsEmpty { get; private set; } = "The history is empty";
        public string ClearHistory { get; private set; } = "Clear History";
        public string RefreshHistory { get; private set; } = "Refresh History";
        public string EnableHistory { get; private set; } = "Enable History";
        public string DisableHistory { get; private set; } = "Disable History";
        public string DragDropText { get; private set; } = "... or simply drop files here!";
        public string RecentFilesLabel { get; private set; } = "Files recently created by PDFCreator:";
        public string RemoveFromHistory { get; private set; } = "Remove from history";
        public string DocumentInfo { get; private set; } = "Document Info";
        public string TitleLabel { get; private set; } = "Title:";
        public string AuthorLabel { get; private set; } = "Author:";
        public string PagesLabel { get; private set; } = "Pages:";
        public string OpenWith { get; private set; } = "Open with";
        public string DeleteFile { get; private set; } = "Delete";
        public string OpenDefaultProgram { get; private set; } = "Open with default viewer";
        public string OpenExplorer { get; private set; } = "Open folder";
        public string PrintWithPDFArchitect { get; private set; } = "Print with PDF Architect";
        public string OpenPDFArchitect { get; private set; } = "Open with PDF Architect";
        public string OpenMailClient { get; private set; } = "Open with E-Mail Client";
        public string HistoryIsDisabled { get; private set; } = "History is disabled";

        public string FormatCallToAction(string printerName)
        {
            return string.Format(CallToAction, printerName);
        }
    }
}
