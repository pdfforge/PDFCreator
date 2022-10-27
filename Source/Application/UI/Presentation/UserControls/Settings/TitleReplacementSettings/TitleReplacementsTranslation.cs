using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.TitleReplacementSettings
{
    public class TitleReplacementsTranslation: ITranslatable
    {
        public string CancelButtonContent { get; private set; } = "Cancel";
        public string EditTextReplacementTitle { get; private set; } = "Edit Title Replacement";
        public string RadioButtonRemoveAll { get; private set; } = "Remove All";
        public string RadioButtonRemoveAtBeginning { get; private set; } = "Remove at beginning";
        public string RadioButtonRemoveAtEnd { get; private set; } = "Remove at end";
        public string RadioButtonReplaceWithRegEx { get; private set; } = "Replace with regular expression";
        public string InvalidRegex { get; private set; } = "Invalid Regular Expression.";
        public string UserGuide { get; private set; } = "Show User Guide";
        public string SearchForText { get; private set; } = "Search for:";
        public string SearchModelText { get; private set; } = "Search mode:";
        public string ReplaceWithText { get; private set; } = "Replace with:";
        public string OkButtonContent { get; private set; } = "Ok";
        public string PreviewTitleText { get; private set; } = "Replaced Title:";
        public string SampleTitleText { get; private set; } = "Sample Title:";
        public string AddTitle { get; private set; } = "Add Title";
        public string TitleReplacementControlHeader { get; private set; } = "Title Replacement";
        public string TitleReplacementPreviewHeader { get; private set; } = "Preview";

        // These properties are only accessed via reflection!
        public string Start { get; private set; } = "Start";
        public string End { get; private set; } = "End";
        public string Replace { get; private set; } = "Replace";
        public string RegEx { get; private set; } = "RegEx";
    }
}
