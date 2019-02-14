using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.Utilities.Process;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeBusinessFeatureControlViewModel : BusinessFeatureControlViewModel
    {
        public DesignTimeBusinessFeatureControlViewModel() : base(new EditionHelper(true, true), new ProcessStarter(), new DesignTimeTranslationUpdater())
        {
        }
    }
}
