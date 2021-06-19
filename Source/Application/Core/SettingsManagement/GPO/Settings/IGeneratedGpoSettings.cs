using pdfforge.DataStorage;

namespace pdfforge.PDFCreator.Core.SettingsManagement.GPO.Settings
{
    public interface IGeneratedGpoSettings
    {
        void ReadValues(Data data, string path = "");
    }
}
