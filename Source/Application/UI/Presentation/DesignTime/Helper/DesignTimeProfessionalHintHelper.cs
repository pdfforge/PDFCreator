using pdfforge.PDFCreator.UI.Presentation.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper
{
    public class DesignTimeProfessionalHintHelper : IProfessionalHintHelper
    {
        public int CurrentJobCounter => 101;

        public bool QueryDisplayHint()
        {
            return true;
        }
    }
}
