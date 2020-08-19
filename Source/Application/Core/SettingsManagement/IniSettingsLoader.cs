using System.Collections.Generic;
using pdfforge.DataStorage;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface IIniSettingsLoader
    {
        ISettings LoadIniSettings(string iniFile);

        int SettingsVersion { set; }
    }

    public class IniSettingsLoader : IIniSettingsLoader
    {
        private readonly IDataStorageFactory _dataStorageFactory;
        private readonly IDefaultSettingsBuilder _settingsBuilder;
        private readonly IMigrationStorageFactory _migrationStorageFactory;
        private readonly ISettingsBackup _settingsBackup;

        public IniSettingsLoader(IDataStorageFactory dataStorageFactory, IDefaultSettingsBuilder settingsBuilder, IMigrationStorageFactory migrationStorageFactory, ISettingsBackup settingsBackup)
        {
            _dataStorageFactory = dataStorageFactory;
            _settingsBuilder = settingsBuilder;
            _migrationStorageFactory = migrationStorageFactory;
            _settingsBackup = settingsBackup;
        }

        public ISettings LoadIniSettings(string iniFile)
        {
            if (string.IsNullOrWhiteSpace(iniFile))
                return null;

            var iniStorage = _dataStorageFactory.BuildIniStorage(iniFile);

            var settings = _settingsBuilder.CreateEmptySettings();
            

            var storage = _migrationStorageFactory.GetMigrationStorage(iniStorage, SettingsVersion, _settingsBackup);

            settings.LoadData(storage);

            return settings;
        }

        public int SettingsVersion { get; set; }
    }
}
