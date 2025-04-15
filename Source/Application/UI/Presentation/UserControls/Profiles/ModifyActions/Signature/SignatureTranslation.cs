using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Signature
{
    public class SignatureTranslation : ActionTranslationBase
    {
        public string SignatureLocationAndSize { get; private set; } = "Position and size";
        public string CertificateFileLabel { get; private set; } = "Certificate file:";
        public string PasswordLabel { get; private set; } = "Password:";
        public string IncorrectPassword { get; private set; } = "The entered password is not valid for this certificate";
        public string FitTextToSignatureSize { get; private set; } = "Fit text to signature size";
        public string AllFiles { get; private set; } = "All files";
        public string PNGFiles { get; private set; } = "PNG files";
        public string ImageFiles { get; private set; } = "Image files";
        public string SelectSignatureImageFile { get; private set; } = "Select signature image file";
        public string ShowImageOnly { get; private set; } = "Show image only";
        public string DontSavePassword { get; private set; } = "Don't save password and request it during conversion (this is not possible for automatic saving)";
        public string ReasonLabel { get; private set; } = "Reason:";
        public string ContactLabel { get; private set; } = "Contact:";
        public string LocationLabel { get; private set; } = "Location:";
        public string AllowMultiSigningCheckBox { get; private set; } = "Allow multiple signing";
        public string DisplayLevelLabel { get; private set; } = "Display signature:";
        public string NoDisplayLabel { get; private set; } = "No display";
        public string DisplayTextLabel { get; private set; } = "Text";
        public string DisplayImageLabel { get; private set; } = "Image";
        public string DisplayImageAndTextLabel { get; private set; } = "Image and text";
        public string SelectTimeServerLabel { get; private set; } = "Select time server:";
        public string DisplaySignatureCheckBox { get; private set; } = "Display signature in document";
        public string SignatureImageFileLabel { get; private set; } = "Signature image:";
        public string DrawCustomSignatureLabel { get; private set; } = "Draw custom signature";
        public string UnitOfMeasurementLabel { get; private set; } = "Unit of measurement:";
        public string FromLeftLabel { get; private set; } = "From left:";
        public string WidthLabel { get; private set; } = "Width:";
        public string FromBottomLabel { get; private set; } = "From bottom:";
        public string HeightLabel { get; private set; } = "Height:";
        public string SelectCertFile { get; private set; } = "Select certificate file";
        public string PfxP12Files { get; private set; } = "PFX/P12 files";
        public string SignaturePasswordButton { get; private set; } = "Set certificate password";
        public string SelectOrAddTimeServerAccount { get; protected set; } = "Select account or create a new one ->";

        public EnumTranslation<SignaturePage>[] SignaturePageValues { get; private set; } = EnumTranslation<SignaturePage>.CreateDefaultEnumTranslation();
        public EnumTranslation<UnitOfMeasurement>[] UnitOfMeasurementValues { get; private set; } = EnumTranslation<UnitOfMeasurement>.CreateDefaultEnumTranslation();

        public override string Title { get; set; } = "Signature";
        public override string InfoText { get; set; } = "Digitally signs PDF files, for example to verify the sender’s identity and to ensure the document has not been changed after signing. Signature and Encryption are always processed last.";

        public string Ok { get; protected set; } = "Ok";
        public string Save { get; protected set; } = "Save as file";
        public string Cancel { get; protected set; } = "Cancel";
        public string Remove { get; protected set; } = "Remove";
        public string Reset { get; protected set; } = "Reset";
        public string Brush { get; protected set; } = "Brush";
    }
}
