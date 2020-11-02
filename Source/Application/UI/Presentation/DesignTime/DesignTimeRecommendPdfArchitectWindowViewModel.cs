using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Windows;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeRecommendPdfArchitectWindowViewModel : RecommendPdfArchitectWindowViewModel
    {
        public DesignTimeRecommendPdfArchitectWindowViewModel() : base(null, null, new DesignTimeTranslationUpdater(), null, null, null)
        {
        }
    }
}
