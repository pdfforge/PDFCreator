using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Conversion.ActionsInterface
{
    public interface IPdfProcessor
    {
        void SignEncryptConvertPdfAAndWriteFile(Job job);

        int GetNumberOfPages(string pdfFile);

        string DeterminePdfVersion(ConversionProfile profile);

        void AddAttachment(Job job);

        void AddCover(Job job);

        void AddStamp(Job job);

        void AddBackground(Job job);

        void AddWatermark(Job job);
    }
}
