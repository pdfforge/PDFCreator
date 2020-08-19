using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeAboutViewModel : AboutViewModel
    {
        public DesignTimeAboutViewModel() : base(new DesignTimeVersionHelper(), new DesignTimeTranslationUpdater(), new DesignTimeCommandLocator(),
            new DesignTimeApplicationNameProvider(), new EditionHelper(true))
        {
        }
    }
}
