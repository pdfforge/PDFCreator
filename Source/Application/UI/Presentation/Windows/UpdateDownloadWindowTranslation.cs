using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.Windows
{
    public class UpdateDownloadWindowTranslation : ITranslatable
    {
        public string CancelButtonContent { get; private set; } = "_Cancel";
        public string LoadingUpdateText { get; private set; } = "Loading Update...";
    }
}
