using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced
{
    public class ProfileAdvancedTranslation : ITranslatable
    {
        public string Advanced { get; private set; } = "Advanced";
        public string UserToken { get; private set; } = "User Tokens";
        public string Script { get; private set; } = "Run Program";
        public string CsScript { get; private set; } = "CS-Script";
        public string Forward { get; private set; } = "Forward";
    }
}
