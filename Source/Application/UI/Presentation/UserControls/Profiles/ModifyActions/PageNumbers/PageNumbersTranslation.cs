using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.ModifyActions
{
    public class PageNumbersTranslation : ActionTranslationBase
    {
        public string FromTopLabel { get; private set; } = "From top:";
        public string FromBottomLabel { get; private set; } = "From bottom:";
        public string FromRightLabel { get; private set; } = "From right:";
        public string FromLeftLabel { get; private set; } = "From left:";
        public string FromCenterLabel { get; private set; } = "From center:";
        public string UnitOfMeasurementLabel { get; private set; } = "Unit of measurement:";
        public string AlternateCorner { get; private set; } = "Alternate corner every page";
        public string NumberPosition { get; private set; } = "Positioning of the number on the page:";
        public string NumberSettings { get; private set; } = "Number Settings";
        public string BeginOnPage { get; private set; } = "Begin on page:";
        public string BeginAtNumber { get; private set; } = "Begin counting at:";
        public string UseRomanNumerals { get; private set; } = "Use Roman numerals";
        public string Format { get; private set; } = "Format:";

        public EnumTranslation<UnitOfMeasurement>[] UnitOfMeasurementValues { get; private set; } = EnumTranslation<UnitOfMeasurement>.CreateDefaultEnumTranslation();
        public EnumTranslation<PageNumberPosition>[] PageNumbersPositionValues { get; private set; } = EnumTranslation<PageNumberPosition>.CreateDefaultEnumTranslation();

        public override string Title { get; set; } = "Page Numbers";
        public override string InfoText { get; set; } = "Adds page numbers to the document.";
    }
}
