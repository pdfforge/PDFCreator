using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    internal class DesignTimeSignaturePasswordStepViewModel : SignaturePasswordStepViewModel
    {
        public DesignTimeSignaturePasswordStepViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeSignaturePasswordCheck())
        {
        }
    }
}
