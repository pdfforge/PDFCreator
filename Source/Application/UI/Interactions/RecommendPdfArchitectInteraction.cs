using pdfforge.Obsidian.Interaction;
using pdfforge.PDFCreator.Conversion.Actions.Queries;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class RecommendPdfArchitectInteraction : IInteraction
    {
        public PdfArchitectRecommendPurpose RecommendPurpose { get; set; }

        public RecommendPdfArchitectInteraction(PdfArchitectRecommendPurpose recommendPurpose)
        {
            RecommendPurpose = recommendPurpose;
        }
    }
}
