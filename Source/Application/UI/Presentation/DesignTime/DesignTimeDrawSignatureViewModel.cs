using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeDrawSignatureViewModel : DrawSignatureViewModel
    {
        public DesignTimeDrawSignatureViewModel() : base(
            new DesignTimeTranslationUpdater(),
            new DesignTimeInteractionRequest(),
            new DesignTimeInteractionInvoker(),
            new DesignTimeCanvasToFileHelper(),
            new DesignTimeCurrentSettingsProvider())
        {
        }
    }
}
