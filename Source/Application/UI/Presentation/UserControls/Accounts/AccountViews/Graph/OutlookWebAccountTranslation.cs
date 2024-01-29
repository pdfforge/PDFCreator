using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Graph
{
    public class OutlookWebAccountTranslation : AccountsTranslation, IActionTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
        public string Title { get; set; } = "Outlook Web Access";
        public string InfoText { get; set; } = "Uploads the document to Outlook Web Access to save as draft or send to recipient.";
        private string[] GraphGetsDisabled { get; set; } = { "The Outlook Web Access action will be disabled for this profile.", "The Outlook Web Access action will be disabled for these profiles." };
        public string RemoveOutlookAccount { get; set; } = "Remove Outlook Web Access Account";

        public string GetOutlookGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, GraphGetsDisabled);
        }
    }
}
