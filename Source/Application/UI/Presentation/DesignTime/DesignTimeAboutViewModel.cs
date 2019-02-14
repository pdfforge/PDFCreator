using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using pdfforge.PDFCreator.Utilities.Threading;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeAboutViewModel : AboutViewModel
    {
        public DesignTimeAboutViewModel() : base(new DesignTimeVersionHelper(), new TranslationUpdater(new TranslationFactory(), new ThreadManager()), new DesignTimeCommandLocator(), new DesignTimeApplicationNameProvider(), null)
        {
        }
    }
}
