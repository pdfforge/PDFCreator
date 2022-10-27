using pdfforge.PDFCreator.Conversion.Settings.Enums;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles
{
    public class OutputFormatTranslation : ITranslatable
    {
        public string OutputFormat { get; private set; } = "Output Format";

        /*Pdf*/
        public string GeneralSettings { get; private set; } = "General Settings";
        public string PageOrientationLabel { get; private set; } = "Page Orientation:";
        public string ColorModelLabel { get; private set; } = "Color Model:";
        public string ViewerSettings { get; private set; } = "Viewer Settings";
        public string PageViewLabel { get; private set; } = "Page view:";
        public string DocumentViewLabel { get; private set; } = "Document view:";
        public string ViewerStartsOnPageLabel { get; private set; } = "Viewer opens on page:";
        public string CompressionLabel { get; private set; } = "Image compression:";
        public string JpegFactorLabel { get; private set; } = "Factor:";
        public string ResampleImagesToDpiLabel { get; private set; } = "Resample images to (DPI):";
        public string AdvancedLabel { get; private set; } = "Advanced:";
        public string EnableValidationReport { get; private set; } = "Save validation report";
        public string NotSupportedHintInfo { get; private set; } = "The rgb color mode is not supported for PDF/X";
        public EnumTranslation<PageOrientation>[] PageOrientationValues { get; private set; } = EnumTranslation<PageOrientation>.CreateDefaultEnumTranslation();
        public EnumTranslation<ColorModel>[] ColorModelValues { get; private set; } = EnumTranslation<ColorModel>.CreateDefaultEnumTranslation();
        public EnumTranslation<PageView>[] PageViewValues { get; private set; } = EnumTranslation<PageView>.CreateDefaultEnumTranslation();
        public EnumTranslation<DocumentView>[] DocumentViewValues { get; private set; } = EnumTranslation<DocumentView>.CreateDefaultEnumTranslation();
        public EnumTranslation<CompressionColorAndGray>[] CompressionColorAndGrayValues { get; private set; } = EnumTranslation<CompressionColorAndGray>.CreateDefaultEnumTranslation();
        public string UseNewGsPdfInterpreter { get; private set; } = "Use new Ghostscript PDF interpreter\n(uncheck if the new version causes issues)";

        /*Images*/
        public string Colors { get; private set; } = "Colors:";
        public string QualityPercent { get; private set; } = "Quality (%):";
        public string ResolutionDpi { get; private set; } = "Resolution (DPI):";
        public EnumTranslation<JpegColor>[] JpegColorValues { get; set; } = EnumTranslation<JpegColor>.CreateDefaultEnumTranslation();
        public EnumTranslation<PngColor>[] PngColorValues { get; set; } = EnumTranslation<PngColor>.CreateDefaultEnumTranslation();
        public EnumTranslation<TiffColor>[] TiffColorValues { get; set; } = EnumTranslation<TiffColor>.CreateDefaultEnumTranslation();

        /*Text*/
        public string TextFormatIntro { get; private set; } = "As the text format is very limited in what can be displayed, there are many ways to create a TXT file from a printed document. You can choose between four different strategies:";
        public string XmlUnicode { get; private set; } = "XML-escaped Unicode along with information regarding the format of the text";
        public string XmlUnicodeMuPdf { get; private set; } = "Same XML output format as above, but attempt processing similar to MuPDF";
        public string TextUnicode { get; private set; } = "Unicode (UCS2) text with byte order mark (BOM) which approximates the original text layout";
        public string TextUtf8 { get; private set; } = "UTF-8 text which approximates the original text layout";
    }
}
