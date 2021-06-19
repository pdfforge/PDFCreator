using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions
{
    public class WatermarkTranslation : ActionTranslationBase
    {
        public string AllFiles { get; private set; } = "All files";
        public string PDFFiles { get; private set; } = "PDF files";
        public string SelectWatermarkFile { get; private set; } = "Select watermark file";

        public string WatermarkFileLabel { get; private set; } = "Watermark File (Only PDF):";
        public string RepetitionLabel { get; private set; } = "Repetition:";
        public string OpacityLabel { get; private set; } = "Opacity:";
        public string FitToPage { get; private set; } = "Fit to page";
        public string WarningIsPdf20 { get; private set; } = "Warning: The selected document is a PDF 2.0 file and can't be added to other documents during the conversion.";
        public EnumTranslation<BackgroundRepetition>[] BackgroundRepetitionValues { get; private set; }

        public override string Title { get; set; } = "Watermark";
        public override string InfoText { get; set; } = "Adds a watermark to the document.";
    }
}
