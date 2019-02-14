using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.Tabs.ConvertTabs
{
    public class ConvertPdfTranslation : ITranslatable
    {
        public string GeneralSettingsHeader { get; private set; } = "General Settings";
        public string PageOrientationLabel { get; private set; } = "Page Orientation:";
        public string ColorModelLabel { get; private set; } = "Color Model:";
        public string ViewerSettingsHeader { get; private set; } = "Viewer Settings";
        public string PageViewLabel { get; private set; } = "Page view:";
        public string DocumentViewLabel { get; private set; } = "Document view:";
        public string ViewerStartsOnPageLabel { get; private set; } = "Viewer opens on page:";

        public string CompressionLabel { get; private set; } = "Image compression:";
        public string ColorAndGrayJpegFactor { get; private set; } = "Factor:";
        public string ColorAndGrayResampleCheckBox { get; private set; } = "Resample images to";
        public string ColorAndGrayDpi { get; private set; } = "DPI";
        public string EnableValidationReport { get; private set; } = "Save validation report";
        public string NotSupportedHintInfo { get; private set; } = "The rgb color mode is not supported for PDF/X";

        public EnumTranslation<PageOrientation>[] PageOrientationValues { get; private set; } = EnumTranslation<PageOrientation>.CreateDefaultEnumTranslation();
        public EnumTranslation<ColorModel>[] ColorModelValues { get; private set; } = EnumTranslation<ColorModel>.CreateDefaultEnumTranslation();
        public EnumTranslation<PageView>[] PageViewValues { get; private set; } = EnumTranslation<PageView>.CreateDefaultEnumTranslation();
        public EnumTranslation<DocumentView>[] DocumentViewValues { get; private set; } = EnumTranslation<DocumentView>.CreateDefaultEnumTranslation();
        public EnumTranslation<CompressionColorAndGray>[] CompressionColorAndGrayValues { get; private set; } = EnumTranslation<CompressionColorAndGray>.CreateDefaultEnumTranslation();
        public EnumTranslation<CompressionMonochrome>[] CompressionMonochromeValues { get; private set; } = EnumTranslation<CompressionMonochrome>.CreateDefaultEnumTranslation();
    }
}
