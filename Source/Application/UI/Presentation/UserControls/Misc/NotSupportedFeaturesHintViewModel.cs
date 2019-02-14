using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public class NotSupportedFeaturesHintViewModel : TranslatableViewModelBase<NotSupportedFeaturesHintTranslation>, IWhitelisted
    {
        public NotSupportedFeaturesHintViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }
    }

    public class DesignTimeNotSupportedFeaturesHintViewModel : NotSupportedFeaturesHintViewModel
    {
        public DesignTimeNotSupportedFeaturesHintViewModel() : base(new DesignTimeTranslationUpdater())
        {
        }
    }
}
