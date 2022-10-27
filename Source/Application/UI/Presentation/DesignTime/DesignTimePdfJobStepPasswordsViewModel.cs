using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeSecurityPasswordsStepViewModel : SecurityPasswordsStepViewModel
    {
        public DesignTimeSecurityPasswordsStepViewModel() : base(new TranslationUpdater(new TranslationFactory(), new ThreadManager()))
        {
        }
    }
}
