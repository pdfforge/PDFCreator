using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls
{
    public class BusinessFeatureTranslation : ITranslatable
    {
        public string BusinessFeature { get; private set; } = "BUSINESS FEATURE";
        public string ProfessionalRequiredHint { get; private set; } = "You need PDFCreator Professional for this feature. Click here for more information.";
        public string BusinessHint { get; private set; } = "Unlock the business features with PDFCreator Professional!";
    }
}
