using SystemInterface.IO;
using Microsoft.Win32;
using NLog;
using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Core.SettingsManagement.GPO.Settings;

namespace pdfforge.PDFCreator.Core.SettingsManagement.GPO
{
    public class GpoReader<T> where T : IGeneratedGpoSettings
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IStorage _hklmStorage;
        private readonly IStorage _hkcuStorage;

        public GpoReader(string applicationRegKey)
        {
            var policyPath = PathSafe.Combine(@"Software\Policies\", applicationRegKey);
            _hklmStorage = new RegistryStorage(RegistryHive.LocalMachine, policyPath);
            _hkcuStorage = new RegistryStorage(RegistryHive.CurrentUser, policyPath);
        }

        internal GpoReader(IStorage hklmStorage, IStorage hkcuStorage)
        {
            _hklmStorage = hklmStorage;
            _hkcuStorage = hkcuStorage;
        }

        public T ReadGpoSettings(T settings)
        {
            var data = Data.CreateDataStorage();
            TryReadData(_hkcuStorage, data);
            TryReadData(_hklmStorage, data);

            settings.ReadValues(data);

            _logger.Info("GpoSettings applied.");

            return settings;
        }

        private void TryReadData(IStorage storage, Data data)
        {
            try
            {
                storage.ReadData(data);
            }
            catch
            {
                _logger.Debug("Policy path does not exist.");
            }
        }
    }
}
