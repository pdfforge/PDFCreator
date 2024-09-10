using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob.QuickActionStep;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class QuickActionStep : WorkflowStepBase
    {
        public override string NavigationUri => nameof(QuickActionView);

        public override bool IsStepRequired(Job job)
        {
            return job.Profile.ShowQuickActions || job.Profile.OneDriveSettings.ShowShareLink || job.Profile.DropboxSettings.ShowShareLink;
        }
    }
}
