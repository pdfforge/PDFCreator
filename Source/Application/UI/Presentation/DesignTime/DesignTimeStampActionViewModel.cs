using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Stamp;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeStampActionViewModel : StampActionViewModel
    {
        public DesignTimeStampActionViewModel() : base(new DesignTimeTranslationUpdater(),
            null, new DesignTimeActionLocator(), new ErrorCodeInterpreter(new TranslationFactory(null)), new DesignTimeTokenViewModelFactory(),
            new DesignTimeCurrentSettingsProvider(), new DesignTimeTokenHelper(), new DesignTimeDefaultSettingsBuilder(),
            new DesignTimeActionOrderHelper(), new DesignTimeFontSelectorControlViewModelFactory())
        {
        }
    }
}
