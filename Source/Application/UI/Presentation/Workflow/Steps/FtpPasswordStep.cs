using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class FtpPasswordStep : WorkflowStepBase
    {
        public override string NavigationUri => nameof(FtpPasswordView);

        public override bool IsStepRequired(Job job)
        {
            if (!job.Profile.Ftp.Enabled)
                return false;

            var ftpAccount = job.Accounts?.FtpAccounts?.FirstOrDefault(x => x.AccountId == job.Profile.Ftp.AccountId);

            if (ftpAccount?.AuthenticationType == AuthenticationType.KeyFileAuthentication)
            {
                return string.IsNullOrEmpty(job.Passwords.FtpPassword)
                       && !string.IsNullOrWhiteSpace(ftpAccount.PrivateKeyFile)
                       && ftpAccount.KeyFileRequiresPass;
            }

            return string.IsNullOrEmpty(job.Passwords.FtpPassword);
        }
    }
}
