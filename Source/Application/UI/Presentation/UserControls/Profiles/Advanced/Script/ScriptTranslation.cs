using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.Script
{
    public class ScriptTranslation : ITranslatable
    {
        public string AllFiles { get; private set; } = "All files";
        public string DisplayName { get; private set; } = "Run program";
        public string ExecutableFiles { get; private set; } = "Executable Files";
        public string SelectScriptTitle { get; private set; } = "Select program";
        public string AdditionalParametersText { get; private set; } = "Additional Program Parameters:";
        public string ScriptCallPreviewText { get; private set; } = "Call Preview:";
        public string ScriptFileText { get; private set; } = "Program File:";
        public string WaitForScriptText { get; private set; } = "Wait until the program has ended";
    }
}
