using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Attachment;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeAttachmentActionViewModel : AttachmentActionViewModel
    {
        public DesignTimeAttachmentActionViewModel() : base(new DesignTimeActionLocator(), new DesignTimeErrorCodeInterpreter(), new DesignTimeTranslationUpdater(),
            new DesignTimeCurrentSettingsProvider(), new DesignTimeTokenHelper(),
            null, null, new DesignTimeSelectFilesUserControlViewModelFactory(), null, new DesignTimeActionOrderHelper())
        {
        }
    }
}
