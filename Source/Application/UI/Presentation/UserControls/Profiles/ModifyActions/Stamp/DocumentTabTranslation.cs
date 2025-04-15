namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Stamp
{
    public class DocumentTabTranslation : ActionTranslationBase
    {
        public string StampControlHeader { get; private set; } = "Stamp";
        public string StampFontAsOutlineCheckBoxContent { get; private set; } = "Show font as outline";
        public string StampFontColorLabelContent { get; private set; } = "Font color:";
        public string StampFontLabelContent { get; private set; } = "Font:";
        public string StampOutlineWidthLabelContent { get; private set; } = "Outline width:";
        public string StampTextLabelContent { get; private set; } = "Stamp text:";
        public string FontFileNotSupported { get; private set; } = "The selected font is not supported. Please select a different font.";
        public override string Title { get; set; } = "Stamp";

        public override string InfoText { get; set; } = "Places a stamp text on the document.";
    }
}
