using Microsoft.Win32;
using pdfforge.DataStorage.Storage;

namespace pdfforge.PDFCreator.Core.SettingsManagementInterface
{
    public interface IDataStorageFactory
    {
        IStorage BuildIniStorage(string file);

        IStorage BuildRegistryStorage(RegistryHive registryHive, string baseKey, bool clearOnWrite = false);
    }
}
