using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeWelcomeViewModel : WelcomeViewModel
    {
        public DesignTimeWelcomeViewModel()
            : base(new DesignTimeCommandLocator(), new DesignTimeTranslationUpdater(),
                new EditionHelper(true), new DesignTimeVersionHelper(), new DesignTimeApplicationNameProvider())
        {
        }
    }
}
