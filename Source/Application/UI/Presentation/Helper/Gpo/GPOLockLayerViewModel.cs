using pdfforge.PDFCreator.Core.ServiceLocator;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Gpo
{
    public class GpoLockLayerViewModel : TranslatableViewModelBase<GpoTranslation>, IWhitelisted
    {
        public GpoLockLayerViewModel(ITranslationUpdater translationUpdater) : base(translationUpdater)
        {
        }
    }

    public class DesignTimeGpoLockLayerViewModel : GpoLockLayerViewModel
    {
        public DesignTimeGpoLockLayerViewModel() : base(new DesignTimeTranslationUpdater())
        {
        }
    }
}
