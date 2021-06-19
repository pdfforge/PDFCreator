using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Stamp;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeStampUserControlViewModel : StampUserControlViewModel
    {
        public DesignTimeStampUserControlViewModel() : base(new DesignTimeTranslationUpdater(),
            null, new DesignTimeActionLocator(), new ErrorCodeInterpreter(new TranslationFactory(null)), null,
            new DesignTimeTokenViewModelFactory(), new DesignTimeCurrentSettingsProvider(),
            null, new DesignTimeTokenHelper(), null, null)
        {
        }
    }
}
