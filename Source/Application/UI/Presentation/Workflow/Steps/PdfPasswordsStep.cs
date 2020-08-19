using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class PdfPasswordsStep : WorkflowStepBase
    {
        public override string NavigationUri => nameof(PdfPasswordView);

        public override bool IsStepRequired(Job job)
        {
            var outputFormat = job.Profile.OutputFormat;

            // PDF/A nad PDF/X can't be encrypted
            if (outputFormat != OutputFormat.Pdf)
                return false;

            var securitySettings = job.Profile.PdfSettings.Security;

            if (!securitySettings.Enabled)
                return false;

            if (securitySettings.OwnerPassword == "")
                return true;

            return securitySettings.RequireUserPassword && securitySettings.UserPassword == "";
        }
    }
}
