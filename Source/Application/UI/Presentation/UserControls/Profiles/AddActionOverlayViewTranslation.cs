using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class AddActionOverlayViewTranslation : ITranslatable
    {
        public string Preparation { get; private set; } = "Preparation";
        public string Modify { get; private set; } = "Modify";
        public string Send { get; private set; } = "Send";
        public string AddAction { get; private set; } = "Choose an Action";
    }
}
