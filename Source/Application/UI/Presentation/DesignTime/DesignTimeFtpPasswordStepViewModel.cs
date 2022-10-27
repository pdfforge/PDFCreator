using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeFtpPasswordStepViewModel : FtpPasswordStepViewModel
    {
        public DesignTimeFtpPasswordStepViewModel() : base(new TranslationUpdater(new TranslationFactory(), new ThreadManager()))
        {
            FtpAccountInfo = "test-user@localhost";
        }
    }
}
