using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public enum MicrosoftAccountPermission
    {
        [StringValue("mail.readwrite")]
        MailReadWrite = 0,

        [StringValue("mail.send")]
        MailSend = 1,

        [StringValue("Files.ReadWrite")]
        FilesReadWrite = 2,
    }

    public static class MicrosoftAccountPermissionExtension
    {

        public static string ToPermissionString(this MicrosoftAccountPermission permission)
        {
            return StringValueAttribute.GetValue(permission);
        }
    }

}
