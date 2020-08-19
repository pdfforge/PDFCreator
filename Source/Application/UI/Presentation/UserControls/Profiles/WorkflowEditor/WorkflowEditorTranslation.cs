using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class WorkflowEditorTranslation : ITranslatable
    {
        public string ExplanationTitle { get; set; } = "Create your custom profile settings";

        public string ExplanationText { get; set; } = @"Select actions for your custom profile by clicking on “Add Action” " +
                                          "and drag them into the order in which you would like them to be executed.";

        public string OutputFormatColon { get; set; } = "Output Format:";
        public string Save { get; set; } = "Save";
        public string MetaDataColon { get; set; } = "Metadata:";
        public string Modify { get; set; } = "Modify";
        public string Preparation { get; set; } = "Preparation";
        public string Send { get; set; } = "Send";
        public string MetadataTab { get; private set; } = "Metadata";
        public string OutputFormat { get; private set; } = "Output Format";
        public string Printer { get; private set; } = "Printer";

        public string AddAction { get; protected set; } = "Add Action";
        public string TargetLocation { get; protected set; } = "Target location:";
        public string LastUsedDirectory { get; protected set; } = "Last used directory";
        public string MissingDirectory { get; protected set; } = "Missing directory";

        public string AutoSaveEnabled { get; protected set; } = "Auto-Save: Enabled";
        public string AutoSaveDisabled { get; protected set; } = "Auto-Save: Disabled";

        public string TitlePreview { get; protected set; } = "Title: {0}";
        public string AuthorPreview { get; protected set; } = "Author: {0}";
        public string SubjectPreview { get; protected set; } = "Subject: {0}";
        public string KeywordsPreview { get; protected set; } = "Keywords: {0}";

        public string GetFormattedTitlePreview(string titlePreview)
        {
            return string.Format(TitlePreview, titlePreview);
        }

        public string GetFormattedAuthorPreview(string authorPreview)
        {
            return string.Format(AuthorPreview, authorPreview);
        }

        public string GetFormattedSubjectPreview(string subjectPreview)
        {
            return string.Format(SubjectPreview, subjectPreview);
        }

        public string GetFormattedKeywordsPreview(string keywordsPreview)
        {
            return string.Format(KeywordsPreview, keywordsPreview);
        }
    }
}
