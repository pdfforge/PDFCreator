using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public class BusinessFeatureControlTranslation : ITranslatable
    {
        public string BusinessFeature { get; private set; } = "BUSINESS FEATURE";
        public string BusinessRequiredHint { get; private set; } = "You need PDFCreator Business for this feature. Click for more information.";
        public string BusinessHint { get; private set; } = "Unlock the premium features with PDFCreator Business!";
    }
}
