using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class RecommendPdfArchitectWindowTranslation : ITranslatable
    {
        public string DownloadButtonContent { get; private set; } = "Install";
        public string ErrorTextInstall { get; private set; } = "No application is associated to open PDF files";
        public string ErrorTextUpdate { get; private set; } = "Your version of PDF Architect is outdated";
        public string InfoButtonContent { get; private set; } = "More info";
        public string NoButtonContent { get; private set; } = "No, thanks";
        public string RecommendTextInstall { get; private set; } = "We recommend PDF Architect, our free PDF Viewer and Editor.\nThe free edition contains a viewer and basic editing capabilities. Furthermore, you can buy optional modules, which extend the functionality according to your requirements and even provide OCR.";
        public string RecommendTextUpdate { get; private set; } = "We recommend installing the new, improved PDF Architect version – without the latest version you will miss out on features like printing directly from PDF Architect.";
    }
}
