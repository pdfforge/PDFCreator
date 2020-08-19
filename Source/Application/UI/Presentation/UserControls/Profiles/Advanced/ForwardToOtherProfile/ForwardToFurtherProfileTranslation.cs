using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced.ForwardToOtherProfile
{
    public class ForwardToFurtherProfileTranslation : ITranslatable
    {
        public string DisplayName { get; private set; } = "Forward to Profile";
        public string TriggerAnotherConversionOfOriginalDocument { get; private set; } = "Trigger another conversion by forwarding the original source document to an other profile, e.g. to convert to another output format.";
        public string SelectProfile { get; private set; } = "Select profile:";
    }
}
