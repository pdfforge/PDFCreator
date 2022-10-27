using System.Windows;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Settings.DebugSettings
{
    public class LoadSpecificProfileTranslation : ITranslatable
    {
        public string Title { get; private set; } = "Load Profiles";
        public string Description { get; private set; } = "Load specific profiles from an exported INI file";
        public string LoadFileLabel { get; private set; } = "Settings INI file:";
        public string SelectIniFile { get; private set; } = "Select INI file";
        public string PdfCreatorSettingsFiles { get; private set; } = "PDFCreator settings files";
        public string AllFiles { get; private set; } = "All files";
        public string AddButton { get; private set; } = "Add to settings";
    }
}
