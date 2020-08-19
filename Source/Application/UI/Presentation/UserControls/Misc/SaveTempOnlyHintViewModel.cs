using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public class SaveTempOnlyHintViewModel : TranslatableViewModelBase<SaveTempHintTranslation>, IWhitelisted
    {
        public SaveTempOnlyHintViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }
    }

    public class DesignTimeSaveTempOnlyHintViewModel : SaveTempOnlyHintViewModel
    {
        public DesignTimeSaveTempOnlyHintViewModel() : base(new DesignTimeTranslationUpdater())
        {
        }
    }
}
