using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.EmailCollectionHintStep;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps;
public class EmailCollectionHintStep : WorkflowStepBase
{
    public override string NavigationUri => nameof(EmailCollectionHintStepView);
    private readonly IConditionalHintManager _conditionalHintManager;

    public EmailCollectionHintStep(IConditionalHintManager conditionalHintManager)
    {
        _conditionalHintManager = conditionalHintManager;
    }
    public override bool IsStepRequired(Job job)
    {
        return _conditionalHintManager.ShouldEmailCollectionHintBeDisplayed();
    }
}
