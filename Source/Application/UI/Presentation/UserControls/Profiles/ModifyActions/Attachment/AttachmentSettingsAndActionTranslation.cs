namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Attachment
{
    public class AttachmentSettingsAndActionTranslation : ActionTranslationBase
    {
        public string AllFiles { get; private set; } = "All files";
        public string DisplayName { get; private set; } = "Add attachment";
        public string PDFFiles { get; private set; } = "PDF files";
        public string SelectAttachmentFile { get; private set; } = "Select attachment file";
        public string AttachmentFileText { get; private set; } = "Attachment files (PDF):";
        public string WarningIsPdf20 { get; private set; } = "Warning: The following documents are PDF 2.0 files and can't be added to other documents during the conversion:";

        public override string Title { get; set; } = "Attachment";
        public override string InfoText { get; set; } = "Appends one or more pages at the end of the document.";
    }
}
