using pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Accounts.AccountViews.Microsoft
{
    public class OneDriveTranslation : AccountsTranslation ,IActionTranslation
    {
        public string EnsureUniqueOneDriveFilename { get; set; } = "Don't overwrite files (add an incrementing number in case a file already exists)";
        public string ShowLink { get; set; } = "Show link at the end of interactive workflow";
        public string PrivateLink { get; set; } = "Private link";
        public string ShareLink { get; set; } = "Share link (anyone with the link has access to it)";
        public string OpenUploadedFile { get; set; } = "Open upload in browser";
        public string Title { get; set; } = "OneDrive";
        public string InfoText { get; set; } = "Upload documents to your OneDrive";
    }
}
