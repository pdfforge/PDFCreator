using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class RecommendPdfArchitectInteraction : IInteraction
    {
        public bool IsUpdate { get; private set; }

        public RecommendPdfArchitectInteraction(bool showViewerWarning, bool isUpdate)
        {
            IsUpdate = isUpdate;
            ShowViewerWarning = showViewerWarning;
        }

        public bool ShowViewerWarning { get; set; }
    }
}
