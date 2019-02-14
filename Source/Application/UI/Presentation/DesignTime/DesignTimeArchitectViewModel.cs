using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Architect;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeArchitectViewModel : ArchitectViewModel
    {
        public DesignTimeArchitectViewModel() : base(null, null, new TranslationUpdater(new TranslationFactory(), new ThreadManager()), null)
        {
        }
    }
}
