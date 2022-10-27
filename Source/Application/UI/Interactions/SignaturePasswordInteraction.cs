namespace pdfforge.PDFCreator.UI.Interactions
{
    public class SignaturePasswordInteraction : BasicPasswordOverlayInteraction
    {
        public SignaturePasswordInteraction(string certificatePath)
            : base(PasswordMiddleButton.Remove)
        {
            CertificatePath = certificatePath;
        }

        public string CertificatePath { get; set; }
    }
}
