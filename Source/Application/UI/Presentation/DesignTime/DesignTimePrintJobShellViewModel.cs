using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimePrintJobShellViewModel : PrintJobShellViewModel
    {
        public DesignTimePrintJobShellViewModel() : base(new DesignTimeApplicationNameProvider(), new InteractionRequest(),
            new DesignTimeTranslationUpdater(), null, new DesignTimeDragAndDropHandler(), new DesignTimeVersionHelper())
        {
        }
    }
}
