using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Advanced
{
    public class ProfileAdvancedTranslation : ITranslatable
    {
        public string Advanced { get; private set; } = "Advanced";
        public string UserToken { get; private set; } = "User Tokens";
        public string Script { get; private set; } = "Script";
        public string CsScript { get; private set; } = "CS-Script";
    }
}
