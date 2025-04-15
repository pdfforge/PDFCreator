using System;
using System.Collections.Generic;
using System.Linq;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    public enum MicrosoftAccountPermission
    {
        [StringValue("Mail.ReadWrite")] MailReadWrite = 0,

        [StringValue("Mail.Send")] MailSend = 1,

        [StringValue("Files.ReadWrite")] FilesReadWrite = 2,

        [StringValue("Sites.Read.All")] SitesReadAll = 3,
        [StringValue("Files.ReadWrite.All")] FilesReadWriteAll = 4,
        [StringValue("Offline_access")] OfflineAccess = 5,
    }

    public static class MicrosoftAccountPermissionExtension
    {
        public static string ToPermissionString(this MicrosoftAccountPermission permission)
        {
            return StringValueAttribute.GetValue(permission);
        }
        public static string ToPermissionScope(this List<MicrosoftAccountPermission> permissions)
        {
            return string.Join(",", permissions.Select(p => p.ToPermissionString()));
        }
        public static bool IsOnlyOfflinePermission(this List<MicrosoftAccountPermission> permissions)
        {
            return permissions.Count == 1 && permissions.Contains(MicrosoftAccountPermission.OfflineAccess);
        }
    }

}
