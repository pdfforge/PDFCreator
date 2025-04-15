namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class HttpPasswordStepTranslation : PasswordButtonControlTranslation
    {
        public string HttpUploadTitle { get; private set; } = "HTTP upload";
        public string Account { get; private set; } = "HTTP account:";
        public string Password { get; private set; } = "HTTP server password:";
    }
}
