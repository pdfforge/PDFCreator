using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class SmtpPasswordStep : WorkflowStepBase
    {
        public override string NavigationUri => nameof(SmtpPasswordStepView);

        public override bool IsStepRequired(Job job)
        {
            if (!job.Profile.EmailSmtpSettings.Enabled)
                return false;

            return string.IsNullOrEmpty(job.Passwords.SmtpPassword);
        }
    }
}
