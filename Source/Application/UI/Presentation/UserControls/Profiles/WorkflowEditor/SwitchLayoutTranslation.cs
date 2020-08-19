using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class SwitchLayoutTranslation : ITranslatable
    {
        public string WarningLayoutSwitchTitle { get; set; } = "Disable workflow editor";
        public string WarningLayoutSwitchCopy { get; set; } = "When turning off the workflow editor, the processing order will be reset to its defaults.";
        public string WantToContinue { get; set; } = "Do you want to continue?";
    }
}
