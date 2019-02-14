using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint;
using pdfforge.PDFCreator.Utilities.Process;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeUpdateHintViewModel : UpdateHintViewModel
    {
        public DesignTimeUpdateHintViewModel() : base(new DesignTimeUpdateAssistant(), new ProcessStarter(),
                                                    new DesignTimeTranslationUpdater(), new EventAggregator(), new DesignTimeVersionHelper(), new DesignTimeUpdateLauncher(), null)
        {
        }
    }
}
