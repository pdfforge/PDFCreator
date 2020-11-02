using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews
{
    public class DropboxTranslation : AccountsTranslation
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
        public string AuthenticateWithDropbox { get; private set; } = "Authenticate with Dropbox";
        public string RemoveDropboxAccount { get; private set; } = "Remove Dropbox Account";
        public string Dropbox { get; private set; } = "Dropbox";
        public string DropboxAccountSeverResponseErrorMessage { get; private set; } = "Incorrect response from Dropbox Server. Account could not be created. Please try again.";
        public string DropboxAccountCreationErrorMessage { get; private set; } = "Dropbox account could not be created. Please try again.";
        public string CreateShareLinkText { get; private set; } = "Create a link to share the uploaded files. Anyone with the link can view this file.";
        public string DropboxAccountSelectContent { get; private set; } = "Please select Dropbox Account:";
        public string DropboxDirectoryLabel { get; private set; } = "Directory:";
        public string EnsureUniqueDropboxFilenamesText { get; private set; } = "Don't overwrite files (adds an increasing number in case of conflict).";
        public string Copy { get; private set; } = "Copy";
        public string CopyShareLinkToClipboard { get; private set; } = "Copy share link to clipboard";
        public string DropboxShareLinkNote { get; private set; } = "Please note: Everyone who receives this link will be able to access the file";
        public string DropboxShareLinkLabel { get; private set; } = "The following link has been created for your document:";

        private string[] DropboxGetsDisabled { get; set; } = { "The Dropbox upload will be disabled for this profile.", "The Dropbox upload will be disabled for this profiles." };

        public string DropboxReturnToApp { get; private set; } = "Thank you for granting access. Please return to PDFCreator.";
        public string DropboxYouCanCloseWindow { get; private set; } = "You can now close this window.";

        public string GetDropboxGetsDisabledMessage(int numberOfProfiles)
        {
            return PluralBuilder.GetFormattedPlural(numberOfProfiles, DropboxGetsDisabled);
        }
    }
}
