using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class DropboxTranslation : AccountsTranslation, IActionTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
        public string AuthenticateWithDropbox { get; set; } = "Authenticate with Dropbox";
        public string RemoveDropboxAccount { get; set; } = "Remove Dropbox Account";
        public string Dropbox { get; set; } = "Dropbox";
        public string Title { get; set; } = "Dropbox";
        public string InfoText { get; set; } = "Uploads the document to Dropbox to save it or to share it with a link.";
        public string DropboxAccountSeverResponseErrorMessage { get; set; } = "Incorrect response from Dropbox Server. Account could not be created. Please try again.";
        public string DropboxAccountCreationErrorMessage { get; set; } = "Dropbox account could not be created. Please try again.";
        public string CreateShareLinkText { get; set; } = "Create a link to share the uploaded files. Anyone with the link can view this file.";
        public string DropboxAccountSelectContent { get; set; } = "Please select Dropbox Account:";
        public string DropboxDirectoryLabel { get; set; } = "Directory:";
        public string EnsureUniqueDropboxFilenamesText { get; set; } = "Don't overwrite files (adds an increasing number in case of conflict).";
        public string Copy { get; set; } = "Copy";
        public string CopyShareLinkToClipboard { get; set; } = "Copy share link to clipboard";
        public string DropboxShareLinkNote { get; set; } = "Please note: Everyone who receives this link will be able to access the file";
        public string DropboxShareLinkLabel { get; set; } = "The following link has been created for your document:";

        private string[] DropboxGetsDisabled { get; set; } = { "The Dropbox upload will be disabled for this profile.", "The Dropbox upload will be disabled for this profiles." };

        public string DropboxReturnToApp { get; set; } = "Thank you for granting access. Please return to PDFCreator.";
        public string DropboxYouCanCloseWindow { get; set; } = "You can now close this window.";

        public string GetDropboxGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, DropboxGetsDisabled);
        }
    }
}
