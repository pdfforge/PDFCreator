using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.MailSmtp;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeEditEmailDifferingFromViewModel : EditEmailDifferingFromViewModel
    {
        public DesignTimeEditEmailDifferingFromViewModel() : base(
            new DesignTimeTranslationUpdater(),
            new DesignTimeTokenViewModelFactory(),
            new DesignTimeEditionHelper())
        { }
    }
}
