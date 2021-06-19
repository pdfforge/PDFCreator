using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Welcome;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeWelcomeViewModel : WelcomeViewModel
    {
        public DesignTimeWelcomeViewModel()
            : base(new DesignTimeCommandLocator(), new DesignTimeTranslationUpdater(),
                new DesignTimeEditionHelper(), new DesignTimeVersionHelper(), new DesignTimeApplicationNameProvider())
        {
        }
    }
}
