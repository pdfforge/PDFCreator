using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow.Steps
{
    public class SignaturePasswordStep : WorkflowStepBase
    {
        public override string NavigationUri => nameof(SignaturePasswordStepView);

        private readonly ISignaturePasswordCheck _signaturePasswordCheck;

        public SignaturePasswordStep(ISignaturePasswordCheck signaturePasswordCheck)
        {
            _signaturePasswordCheck = signaturePasswordCheck;
        }

        public override bool IsStepRequired(Job job)
        {
            if (!job.Profile.OutputFormat.IsPdf())
                return false;

            if (!job.Profile.PdfSettings.Signature.Enabled)
                return false;

            return !_signaturePasswordCheck.IsValidPassword(job.Profile.PdfSettings.Signature.CertificateFile, job.Passwords.PdfSignaturePassword);
        }
    }
}
