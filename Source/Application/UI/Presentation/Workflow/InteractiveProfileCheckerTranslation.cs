using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Workflow
{
    public class InteractiveProfileCheckerTranslation : ITranslatable
    {
        private IPluralBuilder PluralBuilder { get; set; } = new DefaultPluralBuilder();
        public string InvalidSettings { get; private set; } = "Invalid settings";
        public string Error { get; private set; } = "Error";

        public string ConfirmSaveAs { get; private set; } = "Confirm Save As";

        private string FileAlreadyExists { get; set; } = "'{0}' already exists.\nDo you want to replace it?";

        public string GetFileAlreadyExists(string filepath)
        {
            return string.Format(FileAlreadyExists, filepath);
        }
    }
}
