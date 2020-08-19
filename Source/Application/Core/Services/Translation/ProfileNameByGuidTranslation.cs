using Translatable;

namespace pdfforge.PDFCreator.Core.Services.Translation
{
    public class ProfileNameByGuidTranslation : ITranslatable
    {
        public string DefaultGuid { get; private set; } = "<Default Profile>";
        public string HighCompressionGuid { get; private set; } = "High Compression (small file)";
        public string HighQualityGuid { get; private set; } = "High Quality (large file)";
        public string JpegGuid { get; private set; } = "JPEG (graphic file)";
        public string PdfaGuid { get; private set; } = "PDF/A (long term preservation)";
        public string PngGuid { get; private set; } = "PNG (graphic file)";
        public string PrintGuid { get; private set; } = "Print after saving";
        public string TiffGuid { get; private set; } = "TIFF (multipage graphic file)";
        public string SecuredPdfGuid { get; private set; } = "Secured PDF";

        public string GetProfileGuidTranslation(string profileGuid)
        {
            switch (profileGuid)
            {
                case "DefaultGuid":
                    return DefaultGuid;

                case "HighCompressionGuid":
                    return HighCompressionGuid;

                case "HighQualityGuid":
                    return HighQualityGuid;

                case "JpegGuid":
                    return JpegGuid;

                case "PdfaGuid":
                    return PdfaGuid;

                case "PrintGuid":
                    return PrintGuid;

                case "PngGuid":
                    return PngGuid;

                case "TiffGuid":
                    return TiffGuid;

                case "SecuredPdfGuid":
                    return SecuredPdfGuid;
            }

            return "";
        }
    }
}
