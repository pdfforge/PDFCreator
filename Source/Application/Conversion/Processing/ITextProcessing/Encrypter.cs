using iText.Kernel.Pdf;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System.Text;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    public class Encrypter
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        internal void SetEncryption(WriterProperties writerProperties, ConversionProfile profile, JobPasswords jobPasswords)
        {
            if (!profile.PdfSettings.Security.Enabled)
                return;

            var encryption = CalculatePermissionValue(profile);
            _logger.Debug("Calculated Permission Value: " + encryption);

            if (string.IsNullOrEmpty(jobPasswords.PdfOwnerPassword))
            {
                _logger.Error("Launched encryption without owner password.");
                throw new ProcessingException("Launched encryption without owner password.", ErrorCode.Encryption_NoOwnerPassword);
            }

            var ownerPassword = Encoding.Default.GetBytes(jobPasswords.PdfOwnerPassword);

            byte[] userPassword = null;

            if (profile.PdfSettings.Security.RequireUserPassword)
            {
                if (string.IsNullOrEmpty(jobPasswords.PdfUserPassword))
                {
                    _logger.Error("Launched encryption without user password.");
                    throw new ProcessingException("Launched encryption without user password.", ErrorCode.Encryption_NoUserPassword);
                }
                userPassword = Encoding.Default.GetBytes(jobPasswords.PdfUserPassword);
            }

            switch (profile.PdfSettings.Security.EncryptionLevel)
            {
                case EncryptionLevel.Rc40Bit:
                    writerProperties.SetStandardEncryption(userPassword, ownerPassword,
                        encryption, EncryptionConstants.STANDARD_ENCRYPTION_40);
                    break;

                case EncryptionLevel.Rc128Bit:
                    writerProperties.SetStandardEncryption(userPassword, ownerPassword,
                        encryption, EncryptionConstants.STANDARD_ENCRYPTION_128);
                    break;

                case EncryptionLevel.Aes128Bit:
                case EncryptionLevel.Aes256Bit:
                    writerProperties.SetStandardEncryption(userPassword, ownerPassword,
                        encryption, EncryptionConstants.ENCRYPTION_AES_128);
                    break;
            }
        }

        /// <summary>
        ///     Calculates the PDF permission value that results in the settings from the given profile
        /// </summary>
        /// <param name="profile">The profile to do the calculations with.</param>
        /// <returns>An integer that encodes the PDF security permissions</returns>
        private int CalculatePermissionValue(ConversionProfile profile)
        {
            var permissionValue = 0;

            if (profile.PdfSettings.Security.AllowPrinting) permissionValue = permissionValue | EncryptionConstants.ALLOW_PRINTING;
            if (profile.PdfSettings.Security.AllowToEditTheDocument)
                permissionValue = permissionValue | EncryptionConstants.ALLOW_MODIFY_CONTENTS;
            if (profile.PdfSettings.Security.AllowToCopyContent) permissionValue = permissionValue | EncryptionConstants.ALLOW_COPY;
            if (profile.PdfSettings.Security.AllowToEditComments)
                permissionValue = permissionValue | EncryptionConstants.ALLOW_MODIFY_ANNOTATIONS;

            if ((profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Rc128Bit)
                || (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes128Bit)
                || (profile.PdfSettings.Security.EncryptionLevel == EncryptionLevel.Aes256Bit))
            {
                if (profile.PdfSettings.Security.AllowPrinting)
                    if (profile.PdfSettings.Security.RestrictPrintingToLowQuality)
                        permissionValue = permissionValue ^ EncryptionConstants.ALLOW_PRINTING ^ EncryptionConstants.ALLOW_DEGRADED_PRINTING;
                //Remove higher bit of AllowPrinting
                if (profile.PdfSettings.Security.AllowToFillForms)
                    permissionValue = permissionValue | EncryptionConstants.ALLOW_FILL_IN; //Set automatically for 40Bit
                if (profile.PdfSettings.Security.AllowScreenReader)
                    permissionValue = permissionValue | EncryptionConstants.ALLOW_SCREENREADERS; //Set automatically for 40Bit
                if (profile.PdfSettings.Security.AllowToEditAssembly)
                    permissionValue = permissionValue | EncryptionConstants.ALLOW_ASSEMBLY; //Set automatically for 40Bit
            }
            return permissionValue;
        }
    }
}
