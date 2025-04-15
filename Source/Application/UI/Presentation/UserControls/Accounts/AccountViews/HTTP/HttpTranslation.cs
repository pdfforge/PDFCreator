using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class HttpTranslation : AccountsTranslation, IActionTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();

        //ProfileSubTab
        public string HttpSubTabTitle { get; private set; } = "HTTP";

        public string HttpAccountLabel { get; private set; } = "Select HTTP account:";

        //Edit Command
        public string EditHttpAccount { get; private set; } = "Edit HTTP account";

        //Delete Command
        public string RemoveHttpAccount { get; private set; } = "Remove HTTP account";

        public string UrlText { get; set; } = "URL:";
        public string HasBasicAuthenticationText { get; set; } = "Basic authentication";

        private string[] HttpGetsDisabled { get; set; } = { "The HTTP action will be disabled for this profile.", "The HTTP action will be disabled for this profiles." };

        public string GetHttppGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, HttpGetsDisabled);
        }

        public string Title { get; set; } = "HTTP";
        public string InfoText { get; set; } = "Uploads the document to an HTTP server.";
    }
}
