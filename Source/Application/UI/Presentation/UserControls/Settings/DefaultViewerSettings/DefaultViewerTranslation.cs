using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DefaultViewerSettings
{
    public class DefaultViewerTranslation: ITranslatable
    {
        public string DefaultViewerSettingsHeader { get; private set; } = "Default Viewer";
        public string AllFiles { get; private set; } = "All files";
        public string ExecutableFiles { get; private set; } = "Executable files";
        public string ExecutablePath { get; private set; } = "Executable Path:";
        public string Parameters { get; private set; } = "Additional Parameters:";
        public string Description { get; private set; } = "Here you can setup a custom program for viewing your generated documents. The windows default program will be used if you don't select anything here.";
    }
}
