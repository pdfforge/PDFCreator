using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public interface IActionTranslation : ITranslatable
    {
        string Title { get; set; }
        string InfoText { get; set; }
        string RestrictedHint { get; set; }
    }

    public abstract class ActionTranslationBase : IActionTranslation
    {
        public abstract string Title { get; set; }
        public abstract string InfoText { get; set; }
        public string RestrictedHint { get; set; } = "This feature is not supported by the selected output format.";
    }
}
