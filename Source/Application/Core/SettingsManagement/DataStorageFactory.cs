using Microsoft.Win32;
using pdfforge.DataStorage.Storage;
using System.Text;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class DataStorageFactory : IDataStorageFactory
    {
        public IStorage BuildIniStorage(string file)
        {
            return new IniStorage(file ?? "", Encoding.UTF8);
        }

        public IStorage BuildRegistryStorage(RegistryHive registryHive, string baseKey, bool clearOnWrite = false)
        {
            return new RegistryStorage(registryHive, baseKey, clearOnWrite);
        }
    }
}
