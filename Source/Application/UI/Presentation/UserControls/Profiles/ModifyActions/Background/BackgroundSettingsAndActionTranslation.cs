using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions.Background
{
    public class BackgroundSettingsAndActionTranslation : ActionTranslationBase
    {
        public override string Title { get; set; } = "Background";
        public override string InfoText { get; set; } = "Adds a background to the document.";

        public string AllFiles { get; private set; } = "All files";

        public string PDFFiles { get; private set; } = "PDF files";
        public string SelectBackgroundFile { get; private set; } = "Select background file";

        public string BackgroundFileLabelContent { get; private set; } = "Background File (Only PDF):";
        public string BackgroundRepetitionLabelContent { get; private set; } = "Repetition:";
        public string OpacityLabel { get; private set; } = "Opacity:";
        public string FitToPage { get; private set; } = "Fit to page";
        public string WarningIsPdf20 { get; private set; } = "Warning: The selected document is a PDF 2.0 file and can't be added to other documents during the conversion.";

        public EnumTranslation<BackgroundRepetition>[] BackgroundRepetitionValues { get; private set; }
    }
}
