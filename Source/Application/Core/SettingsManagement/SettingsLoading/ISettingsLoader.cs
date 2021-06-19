using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading
{
    public interface ISettingsLoader
    {
        PdfCreatorSettings LoadPdfCreatorSettings();

        void SaveSettingsInRegistry(PdfCreatorSettings settings);
    }
}
