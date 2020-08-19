using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.Assistants;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.UpdateHint;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class UpdateHintStep : WorkflowStepBase
    {
        private readonly IUpdateHelper _updateHelper;
        public override string NavigationUri => nameof(UpdateHintView);

        public UpdateHintStep(IUpdateHelper updateHelper)
        {
            _updateHelper = updateHelper;
        }

        public override bool IsStepRequired(Job job)
        {
            return _updateHelper.UpdateShouldBeShown();
        }
    }
}
