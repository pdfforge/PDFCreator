using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeHttpPasswordStepViewModel : HttpPasswordStepViewModel
    {
        public DesignTimeHttpPasswordStepViewModel() : base(new TranslationUpdater(new TranslationFactory(), new ThreadManager()))
        {
            HttpAccountInfo = "Hier könnte Ihr Http-Account stehen.";
        }
    }
}
