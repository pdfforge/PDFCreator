using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeBusinessFeatureBadgeViewModel : BusinessFeatureBadgeViewModel
    {
        public DesignTimeBusinessFeatureBadgeViewModel() : base(new EditionHelper(true), null, new DesignTimeTranslationUpdater())
        {
        }
    }
}
