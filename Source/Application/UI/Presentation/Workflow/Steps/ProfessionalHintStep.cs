using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.ProfessionalHintStep;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class ProfessionalHintStep : WorkflowStepBase
    {
        public override string NavigationUri => nameof(ProfessionalHintStepView);

        private readonly IConditionalHintManager _conditionalHintManager;

        public ProfessionalHintStep(IConditionalHintManager conditionalHintManager)
        {
            _conditionalHintManager = conditionalHintManager;
        }

        public override bool IsStepRequired(Job job)
        {
            return _conditionalHintManager.ShouldProfessionalHintBeDisplayed();
        }
    }
}
