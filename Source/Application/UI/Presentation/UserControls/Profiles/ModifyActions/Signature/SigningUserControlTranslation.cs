using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    public class SigningUserControlTranslation : ActionTranslationBase
    {
        public string SignatureTabHeader { get; private set; } = "Signature";
        public string CertificateFileLabel { get; private set; } = "Certificate File:";

        public string DontSavePassword { get; private set; } = "Don't save password and request it during conversion (this is not possible for automatic saving)";
        public string ReasonLabel { get; private set; } = "Reason:";
        public string ContactLabel { get; private set; } = "Contact:";
        public string LocationLabel { get; private set; } = "Location:";
        public string AllowMultiSigningCheckBox { get; private set; } = "Allow multiple signing";
        public string SelectTimeServerLabel { get; private set; } = "Select Time Server:";
        public string DisplaySignatureCheckBox { get; private set; } = "Display signature in document";
        public string SignaturePageLabel { get; private set; } = "Page:";
        public string UnitOfMeasurementLabel { get; private set; } = "Unit of measurement:";
        public string LeftXLabel { get; private set; } = "From left:";
        public string RightXLabel { get; private set; } = "Width:";
        public string LeftYLabel { get; private set; } = "From bottom:";
        public string RightYLabel { get; private set; } = "Height:";
        public string SelectCertFile { get; private set; } = "Select certificate file";
        public string PfxP12Files { get; private set; } = "PFX/P12 files";
        public string AllFiles { get; private set; } = "All files";
        public string SignaturePasswordButton { get; private set; } = "Set certificate password";

        public string SelectOrAddTimeServerAccount { get; protected set; } = "Select account or create a new one ->";

        public EnumTranslation<SignaturePage>[] SignaturePageValues { get; private set; } = EnumTranslation<SignaturePage>.CreateDefaultEnumTranslation();
        public EnumTranslation<UnitOfMeasurement>[] UnitOfMeasurementValues { get; private set; } = EnumTranslation<UnitOfMeasurement>.CreateDefaultEnumTranslation();

        public override string Title { get; set; } = "Signature";
        public override string InfoText { get; set; } = "Digitally signs PDF files, for example to verify the sender’s identity and to ensure the document has not been changed after signing. Signature and Encryption are always processed last.";
    }
}
