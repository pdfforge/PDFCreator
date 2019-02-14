using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.UI.Presentation.Wrapper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeProgressViewModel : ProgressViewModel
    {
        public DesignTimeProgressViewModel()
            : base(null, new InteractionRequest(), new DispatcherWrapper(), new DesignTimeTranslationUpdater(), null)
        {
        }
    }
}
