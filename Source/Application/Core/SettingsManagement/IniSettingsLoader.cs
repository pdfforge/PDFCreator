using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface IIniSettingsLoader
    {
        ISettings LoadIniSettings(string iniFile);
    }

    public class IniSettingsLoader : IIniSettingsLoader
    {
        private readonly IDataStorageFactory _dataStorageFactory;
        private readonly IDefaultSettingsBuilder _settingsBuilder;
        private readonly IMigrationStorageFactory _migrationStorageFactory;

        public IniSettingsLoader(IDataStorageFactory dataStorageFactory, IDefaultSettingsBuilder settingsBuilder, IMigrationStorageFactory migrationStorageFactory)
        {
            _dataStorageFactory = dataStorageFactory;
            _settingsBuilder = settingsBuilder;
            _migrationStorageFactory = migrationStorageFactory;
        }

        public ISettings LoadIniSettings(string iniFile)
        {
            if (string.IsNullOrWhiteSpace(iniFile))
                return null;

            var iniStorage = _dataStorageFactory.BuildIniStorage(iniFile);

            var settings = _settingsBuilder.CreateEmptySettings();

            var storage = _migrationStorageFactory.GetMigrationStorage(iniStorage, CreatorAppSettings.ApplicationSettingsVersion);

            settings.LoadData(storage);

            return settings;
        }
    }
}
