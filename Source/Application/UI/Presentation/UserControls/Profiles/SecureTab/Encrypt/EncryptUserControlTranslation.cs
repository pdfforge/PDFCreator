using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SecureTab.Encrypt
{
    public class EncryptUserControlTranslation : ITranslatable
    {
        public string UserPasswordCheckBox { get; private set; } = "Require a password to open the PDF document";
        public string SecurityPasswordsButton { get; private set; } = "Set Passwords";
        public string AutosaveRequiresPasswords { get; private set; } = "Automatic saving requires the passwords to be set";

        public string EncryptionLevelHeader { get; private set; } = "Encryption Level";
        public string Rc128BitEncryptionText { get; private set; } = "Low (128 Bit)";
        public string Rc128BitEncryptionHint { get; private set; } = "This is not considered secure anymore. Use this for compatibility only.";
        public string Aes128BitEncryptionText { get; private set; } = "Medium (128 Bit AES)";
        public string Aes256BitEncryptionText { get; private set; } = "High (256 Bit AES)";
        public string AllowTheUser { get; private set; } = "Allow the user";
        public string CopyContentPermissionCheckBox { get; private set; } = "to copy content from the document";
        public string PrintDocumentPermissionCheckbox { get; private set; } = "to print the document";
        public string LowQualityPrintPermissionCheckBox { get; private set; } = "restrict to low quality";
        public string ScreenReaderPermissionCheckBox { get; private set; } = "to use a screen reader";
        public string EditDocumentPermissionCheckBox { get; private set; } = "to edit the document";
        public string EditCommentsPermissionCheckBox { get; private set; } = "to edit comments";
        public string FillFormsPermissionCheckBox { get; private set; } = "to fill forms";
        public string EditAssemblyPermissionCheckBox { get; private set; } = "to edit the assembly";
        public string PasswordTitle { get; private set; } = "Password";

        public string GetEncryptionName(EncryptionLevel encryptionLevel)
        {
            switch (encryptionLevel)
            {
                case EncryptionLevel.Aes128Bit:
                    return Aes128BitEncryptionText;

                case EncryptionLevel.Aes256Bit:
                    return Aes256BitEncryptionText;

                case EncryptionLevel.Rc128Bit:
                    return Rc128BitEncryptionText;

                default: throw new Exception($"The encryption level {encryptionLevel} is unknown here");
            }
        }
    }
}
