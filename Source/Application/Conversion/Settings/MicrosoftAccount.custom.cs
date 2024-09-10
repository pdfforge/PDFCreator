using pdfforge.DataStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Conversion.Settings
{
    partial class MicrosoftAccount
    {
        public MicrosoftAccount() { }

        public void CopyTo(MicrosoftAccount targetAccount)
        {
            var data = Data.CreateDataStorage();
            StoreValues(data, "");
            targetAccount.ReadValues(data, "");
        }

        public bool HasPermissions(params MicrosoftAccountPermission[] permissions)
        {
            foreach (var accountPermission in permissions)
            {
                if (!PermissionScopes.Contains(accountPermission.ToPermissionString()))
                    return false;
            }

            return true;
        }

        public DateTime GetExpirationDateTime()
        {
            return DateTimeOffset.FromUnixTimeSeconds(ExpirationDate).DateTime;
        }
    }
}
