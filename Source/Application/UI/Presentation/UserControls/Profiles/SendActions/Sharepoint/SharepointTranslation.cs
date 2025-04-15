namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.Sharepoint
{
    public class SharepointTranslation : IActionTranslation
    {
        public string DirectoryLabel { get; set; } = "Directory:";
        public string SitesLabel { get; set; } = "Sites:";
        public string DriveLabel { get; set; } = "Drives:";
        public string SelectedAccountLabel { get; set; } = "Selected account:";
        public string EnsureUniqueFilename { get; set; } = "Don't overwrite files (add an incrementing number in case a file already exists)";
        public string ShowLink { get; set; } = "Show link at the end of interactive workflow";
        public string OpenUploadedFile { get; set; } = "Open upload in browser";
        public string Title { get; set; } = "SharePoint";
        public string InfoText { get; set; } = "Upload documents to your SharePoint";
        public string RestrictedHint { get; set; } = "This feature is not supported by the selected output format";
        public string EnabledHint { get; set; } = "This feature is already enabled for the selected profile";
        public string SelectOrAddAccount { get; protected set; } = "Select an account or create a new one ->";
    }
}
