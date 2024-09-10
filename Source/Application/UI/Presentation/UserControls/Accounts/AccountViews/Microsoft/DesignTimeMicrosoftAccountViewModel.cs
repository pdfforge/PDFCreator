using pdfforge.PDFCreator.UI.Presentation.DesignTime;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft
{
    public class DesignTimeMicrosoftAccountViewModel:MicrosoftAccountViewModel
    {
        public DesignTimeMicrosoftAccountViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCommandLocator(), null)
        {
        }
    }
}
