using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.ViewModels.Helper
{
    public class ProfessionalHintHelperDisabled : IProfessionalHintHelper
    {
        public int CurrentJobCounter => 0;

        public bool QueryDisplayHint()
        {
            return false;
        }
    }
}
