using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.WorkflowQuery
{
    public class InteractiveWorkflowTranslation : ITranslatable
    {
        public string AnErrorOccured { get; private set; } = "An error occurred:";
        public string Error { get; private set; } = "Error";
        public string JpegFile { get; private set; } = "JPEG file";
        public string PdfA1bFile { get; private set; } = "PDF/A-1b file";
        public string PdfA2bFile { get; private set; } = "PDF/A-2b file";
        public string PdfA3bFile { get; private set; } = "PDF/A-3b file";
        public string PdfFile { get; private set; } = "PDF file";
        public string PdfXFile { get; private set; } = "PDF/X file";
        public string PngFile { get; private set; } = "PNG file";
        private string InvalidRootedPathMessage { get; set; } = "The path '{0}' is not a valid absolute path.\nPlease select another filename and try again.";
        private string CopyErrorMessage { get; set; } = "The file '{0}' could not be saved. Maybe the file is currently in use or you do not have the required permissions.\nPlease select another filename and try again.";
        public string SelectDestination { get; private set; } = "Select destination";
        public string TextFile { get; private set; } = "Text file";
        public string TiffFile { get; private set; } = "TIFF file";

        public string FormatInvalidRootedPathMessage(string filePath)
        {
            return string.Format(InvalidRootedPathMessage, filePath);
        }

        public string FormatCopyErrorMessage(string filePath)
        {
            return string.Format(CopyErrorMessage, filePath);
        }
    }
}
