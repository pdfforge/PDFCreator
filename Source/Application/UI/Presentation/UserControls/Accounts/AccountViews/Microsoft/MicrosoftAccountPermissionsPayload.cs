using System.Collections.Generic;
using pdfforge.PDFCreator.Conversion.Actions.Actions.Mail;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft
{
    public class MicrosoftAccountPermissionsPayload
    {
        public MicrosoftAccount Account;
        public List<MicrosoftAccountPermission> Permissions;

        public MicrosoftAccountPermissionsPayload(MicrosoftAccount account, List<MicrosoftAccountPermission> permissions)
        {
            Account = account;
            Permissions = permissions;
        }
    }
}
