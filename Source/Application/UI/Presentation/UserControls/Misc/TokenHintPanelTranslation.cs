using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public class TokenHintPanelTranslation : ITranslatable
    {
        public string InsecureTokenText { get; set; } = "Click here for more information on tokens";
        public string UserTokenText { get; set; } = "User Tokens need to be activated first";
        public string TokenHintText { get; set; } = "Add Token";
    }
}
