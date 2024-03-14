using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Misc
{
    public class TokenHintPanelTranslation : ITranslatable
    {
        public string InsecureTokenText { get; set; } = "Click here for more information on tokens";
        public string UserTokenNeedsToBeActivated { get; set; } = "The usage of user tokens requires the user token action to be enabled";
        public string AddToken { get; set; } = "Add Token";
    }
}
