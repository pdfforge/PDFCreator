using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.UserToken;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUserTokenActionViewModel : UserTokenActionViewModel
    {
        public DesignTimeUserTokenActionViewModel()
            : base(new DesignTimeTranslationUpdater(),
                null,
                null,
                new DesignTimeActionLocator(),
                new DesignTimeErrorCodeInterpreter(),
                new DesignTimeCurrentSettingsProvider(),
                null,
                null)
        {
        }
    }
}
