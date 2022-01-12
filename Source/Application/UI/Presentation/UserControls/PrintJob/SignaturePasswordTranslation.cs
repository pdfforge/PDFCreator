namespace pdfforge.PDFCreator.UI.Presentation.UserControls.PrintJob
{
    public class SignaturePasswordTranslation : PdfPasswordTranslation
    {
        public string SignaturePasswordTitle { get; private set; } = "Signature";
        public string CertificatePassword { get; private set; } = "Certificate _Password:";
        public string CertificateFile { get; private set; } = "Certificate File:";
        public string IncorrectPassword { get; private set; } = "The entered password is not valid for this certificate";
    }
}
