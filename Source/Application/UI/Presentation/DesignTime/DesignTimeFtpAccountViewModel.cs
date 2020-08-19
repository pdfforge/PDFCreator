using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Tokens;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeFtpAccountViewModel : FtpAccountViewModel
    {
        public DesignTimeFtpAccountViewModel() : base(new DesignTimeTranslationUpdater(), new OpenFileInteractionHelper(new InteractionInvoker()),
            new TokenHelper(new DesignTimeTranslationUpdater()), new DesignTimeTokenViewModelFactory())
        {
        }
    }
}
