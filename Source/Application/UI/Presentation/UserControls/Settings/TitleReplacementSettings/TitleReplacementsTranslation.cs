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
        public string SampleTitleText { get; private set; } = "Sample title:";
        public string PreviewTitleText { get; private set; } = "Sample title after replacements:";
        public string AddTitleReplacement { get; private set; } = "Add title replacement";
        public string TitleReplacementControlHeader { get; private set; } = "Title Replacements";

        public string TitleReplacementDescription { get; private set; } =
            "Some document titles may contain unwanted additions or do not have the right formatting. For example, when printing to PDFCreator, some applications add their name to the document title of the print job.\n"
            + "The list below defines how to handle these additions. The title with the applied title replacements can be accessed via the <PrintJobName> token, which can be used for settings such as output filename, title, etc.";


        // These properties are only accessed via reflection!
        public string Start { get; private set; } = "Start";
        public string End { get; private set; } = "End";
        public string Replace { get; private set; } = "Remove";
        public string RegEx { get; private set; } = "RegEx";
    }
}
