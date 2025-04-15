using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.EmailCollectionHintStep;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime;
public class DesignTimeEmailCollectionStepHintViewModel : EmailCollectionHintStepViewModel
{
    public DesignTimeEmailCollectionStepHintViewModel() : base(new DesignTimeTranslationUpdater(), new DesignTimeCommandLocator(), new DesignTimeConditionalHintManager(), new DesignTimeInteractionRequest())
    {
    }
}
