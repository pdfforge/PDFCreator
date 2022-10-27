using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class HttpPasswordStep : WorkflowStepBase
    {
        public override string NavigationUri => nameof(HttpPasswordStepView);

        public override bool IsStepRequired(Job job)
        {
            if (!job.Profile.HttpSettings.Enabled)
                return false;

            var account = job.Accounts.GetHttpAccount(job.Profile);

            if (!account.IsBasicAuthentication)
                return false;

            return string.IsNullOrEmpty(job.Passwords.HttpPassword);
        }
    }
}
