using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class RecommendPdfArchitectWindowTranslation : ITranslatable
    {
        public string NoApplicationAssociatedWithPdfFiles { get; private set; } = "No application is associated to open PDF files.";

        public string WeRecommendPdfArchitect { get; private set; } = "We recommend PDF Architect, our PDF viewer and editor.";

        public string ArchitectVersionDoesNotSupportThisFeature { get; private set; } = "Your version of PDF Architect does not support this feature.";
        public string WeRecommendNewArchitectVersion { get; private set; } = "We recommend installing the new, improved PDF Architect.";

        public string BenefitFromFreeFeatures { get; private set; } = "Benefit from our free features:";
        public string ViewAndPrint { get; private set; } = "View and print any PDF";
        public string CreatePdfFiles { get; private set; } = "Create PDF files";
        public string InfoButtonContent { get; private set; } = "Read more";
        public string DownloadButtonContent { get; private set; } = "Install";
        public string DontShowAgain { get; private set; } = "Don't show this window again";
        public string MergePdfFiles { get; private set; } = "Merge files";
    }
}
