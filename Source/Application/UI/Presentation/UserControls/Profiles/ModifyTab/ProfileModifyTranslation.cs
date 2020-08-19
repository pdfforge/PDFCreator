using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyTab
{
    public class ProfileModifyTranslation : ITranslatable
    {
        public string Modify { get; private set; } = "Modify";
        public string Background { get; private set; } = "Background";
        public string Watermark { get; private set; } = "Watermark";
        public string Cover { get; private set; } = "Cover";
        public string Stamp { get; private set; } = "Stamp";
        public string Attachment { get; private set; } = "Attachment";
    }
}
