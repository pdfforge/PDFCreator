using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class OutputFormatTranslation : ITranslatable
    {
        public string OutputFormat { get; private set; } = "Output Format";

        /*Pdf*/
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

        /*Jpg*/
        public string JpegColorsLabelContent { get; private set; } = "Colors:";
        public string JpegControlHeader { get; private set; } = "JPEG Settings";
        public string JpegQualityLabelContent { get; private set; } = "Quality (%):";
        public string JpegResolutionLabelContent { get; private set; } = "Resolution (DPI):";
        public EnumTranslation<JpegColor>[] JpegColorValues { get; set; } = EnumTranslation<JpegColor>.CreateDefaultEnumTranslation();

        /*Png*/
        public string PngColorsLabelContent { get; private set; } = "Colors:";
        public string PngControlHeader { get; private set; } = "PNG Settings";
        public string PngResolutionLabelContent { get; private set; } = "Resolution (DPI):";
        public EnumTranslation<PngColor>[] PngColorValues { get; set; } = EnumTranslation<PngColor>.CreateDefaultEnumTranslation();

        /*Tiff*/
        public string TiffColorsLabelContent { get; private set; } = "Colors:";
        public string TiffControlHeader { get; private set; } = "TIFF Settings";
        public string TiffResolutionLabelContent { get; private set; } = "Resolution (DPI):";
        public EnumTranslation<TiffColor>[] TiffColorValues { get; set; } = EnumTranslation<TiffColor>.CreateDefaultEnumTranslation();

        /*Text*/
        public string TextSettingsHeader { get; private set; } = "Text Settings";
        public string TextFormatIntro { get; private set; } = "As the text format is very limited in what can be displayed, there are many ways to create a TXT file from a printed document. You can choose between four different strategies:";
        public string XmlUnicode { get; private set; } = "XML-escaped Unicode along with information regarding the format of the text";
        public string XmlUnicodeMuPdf { get; private set; } = "Same XML output format as above, but attempt processing similar to MuPDF";
        public string TextUnicode { get; private set; } = "Unicode (UCS2) text with byte order mark (BOM) which approximates the original text layout";
        public string TextUtf8 { get; private set; } = "UTF-8 text which approximates the original text layout";
    }
}
