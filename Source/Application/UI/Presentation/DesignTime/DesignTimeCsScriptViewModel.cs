using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.PreparationActions.CsScript;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeCsScriptViewModel : CsScriptViewModel
    {
        public DesignTimeCsScriptViewModel() : base(
            new TranslationUpdater(new TranslationFactory(), new ThreadManager()),
            new DesignTimeDispatcher(),
            null,
            null,
            new DesignTimeErrorCodeInterpreter(),
            new DesignTimeCommandLocator(),
            new DesignTimeActionLocator(),
            new DesignTimeCurrentSettingsProvider(),
            null,
            null)
        { }
    }
}
