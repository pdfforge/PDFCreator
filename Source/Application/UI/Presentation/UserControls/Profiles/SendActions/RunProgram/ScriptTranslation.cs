namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SendActions.RunProgram
{
    public class ScriptTranslation : ActionTranslationBase
    {
        public string AllFiles { get; private set; } = "All files";
        public string DisplayName { get; private set; } = "Run program";
        public string ExecutableFiles { get; private set; } = "Executable Files";
        public string SelectScriptTitle { get; private set; } = "Select program";
        public string AdditionalParametersText { get; private set; } = "Additional Program Parameters:";
        public string ScriptCallPreviewText { get; private set; } = "Call Preview:";
        public string ScriptFileText { get; private set; } = "Program File:";
        public string WaitForScriptText { get; private set; } = "Wait until the program has ended";
        public override string Title { get; set; } = "Run Program";

        public override string InfoText { get; set; } = "Calls a program or script that further processes the document after the conversion. " +
                                                        "PDFCreator calls the specified program automatically and transfers the created files as parameter.";
    }
}
