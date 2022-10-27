using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeSignaturePositionAndSizeViewModel : SignaturePositionAndSizeViewModel
    {
        public DesignTimeSignaturePositionAndSizeViewModel() :
            base(new DesignTimeTranslationUpdater(),
                null)
        { }
    }
}
