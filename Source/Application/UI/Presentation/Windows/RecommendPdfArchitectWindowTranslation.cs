using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class RecommendPdfArchitectWindowTranslation : ITranslatable
    {
        public string ErrorTextNoPdfViewer { get; private set; } = "No application is associated to open PDF files.";
        public string RecommendTextNoPdfViewer { get; private set; } = "We recommend PDF Architect, our PDF viewer and editor.";

        public string ErrorTextNotInstalled { get; private set; } = "PDF Architect could not be found.";
        public string RecommendTextNotInstall { get; private set; } = "Please install PDF Architect, our PDF viewer and editor.";

        public string ErrorTextUpdateRequired { get; private set; } = "Your version of PDF Architect does not support this feature.";
        public string RecommendTextUpdateRequired { get; private set; } = "We recommend installing the new, improved PDF Architect.";

        public string EnjoyFreeFeatures { get; private set; } = "Benefit from our free features:";
        public string ViewAndPrint { get; private set; } = "View and print any PDF";
        public string CreatePdfFiles { get; private set; } = "Create PDF files";
        public string SplitAndMerge { get; private set; } = "Split and merge files";
        public string InfoButtonContent { get; private set; } = "More info";
        public string DownloadButtonContent { get; private set; } = "Install";
    }
}
