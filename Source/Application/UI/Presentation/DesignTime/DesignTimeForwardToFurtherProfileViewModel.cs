using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.ForwardToOtherProfile;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeForwardToFurtherProfileViewModel : ForwardToFurtherProfileViewModel
    {
        public DesignTimeForwardToFurtherProfileViewModel()
        : base(new DesignTimeTranslationUpdater(),
            new DesignTimeDispatcher(),
            new DesignTimeEditionHelper(),
            new DesignTimeActionLocator(),
            new ErrorCodeInterpreter(new TranslationFactory()),
            new DesignTimeCurrentSettingsProvider(),
            new DesignTimeDefaultSettingsBuilder(),
            new DesignTimeActionOrderHelper(true, false))
        {
        }
    }
}
