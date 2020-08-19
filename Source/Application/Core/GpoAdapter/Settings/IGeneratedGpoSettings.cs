using pdfforge.DataStorage;

namespace pdfforge.PDFCreator.Core.GpoAdapter.Settings
{
    public interface IGeneratedGpoSettings
    {
        void ReadValues(Data data, string path = "");
    }
}
