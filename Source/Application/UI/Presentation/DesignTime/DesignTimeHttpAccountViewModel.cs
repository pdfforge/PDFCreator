using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeHttpAccountViewModel : HttpAccountViewModel
    {
        public DesignTimeHttpAccountViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeTokenViewModelFactory(), null)
        {
        }
    }
}
