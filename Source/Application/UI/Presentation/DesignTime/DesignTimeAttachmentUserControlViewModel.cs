using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Attachment;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeAttachmentUserControlViewModel : AttachmentUserControlViewModel
    {
        public DesignTimeAttachmentUserControlViewModel() : base(new DesignTimeActionLocator(), new DesignTimeErrorCodeInterpreter(), new DesignTimeTranslationUpdater(),
            new DesignTimeCurrentSettingsProvider(), new DesignTimeTokenHelper(),
            null, null, new DesignTimeSelectFilesUserControlViewModelFactory(), null, null)
        {
        }
    }
}
