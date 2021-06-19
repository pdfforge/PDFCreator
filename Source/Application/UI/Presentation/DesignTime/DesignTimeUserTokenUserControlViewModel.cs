using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.UserToken;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUserTokenUserControlViewModel : UserTokenUserControlViewModel
    {
        public DesignTimeUserTokenUserControlViewModel()
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
