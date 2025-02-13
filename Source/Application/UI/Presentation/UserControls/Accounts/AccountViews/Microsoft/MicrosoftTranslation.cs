using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft
{
    public class MicrosoftTranslation : AccountsTranslation, IActionTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
        public string Title { get; set; } = "Microsoft";
        public string InfoText { get; set; } = "Uploads the document to Outlook Web Access to save as draft or send to recipient.";
        private string[] GraphGetsDisabled { get; set; } = { "The actions using this account will be disabled for this profile.", "The actions using this account will be disabled for these profiles." };
        public string RemoveOutlookAccount { get; set; } = "Remove Microsoft Account";
        public string EditMicrosoftAccount { get; set; } = "Edit Microsoft Account";
        public string SelectPermissions { get; set; } = "Please choose the permissions you want to grant PDFCreator";
        public string PermissionsExpired { get; set; } = "The permissions for your Microsoft account have expired. Please request them again.";
        public string AccountLogInHint { get; set; } = "You will be asked to sign in to your Microsoft account.";
        public string RevokePermissionsNote { get; set; } = "Note: Granted permissions can only be revoked via the Microsoft portal.";


        public string GetOutlookGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, GraphGetsDisabled);
        }
    }
}
