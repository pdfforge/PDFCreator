using Microsoft.Win32;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using System;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class InitializeDefaultSettingsStart : AppStartBase
    {
        private readonly IIniSettingsLoader _iniSettingsLoader;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IInstallationPathProvider _pathProvider;
        private readonly IDataStorageFactory _storageFactory;

        public string SettingsFile { get; set; }

        public InitializeDefaultSettingsStart(ICheckAllStartupConditions checkAllStartupConditions, IIniSettingsLoader iniSettingsLoader, ISettingsProvider settingsProvider, IInstallationPathProvider pathProvider, IDataStorageFactory storageFactory)
            : base(checkAllStartupConditions)
        {
            _iniSettingsLoader = iniSettingsLoader;
            _settingsProvider = settingsProvider;
            _pathProvider = pathProvider;
            _storageFactory = storageFactory;
        }

        public override ExitCode Run()
        {
            var settings = _iniSettingsLoader.LoadIniSettings(SettingsFile);
            if (settings == null)
                return ExitCode.InvalidSettingsFile;

            if (!_settingsProvider.CheckValidSettings(settings as PdfCreatorSettings))
                return ExitCode.InvalidSettingsInGivenFile;

            try
            {
                var storage = _storageFactory.BuildRegistryStorage(RegistryHive.Users, ".Default\\" + _pathProvider.SettingsRegistryPath);
                settings.SaveData(storage);
            }
            catch (Exception)
            {
                return ExitCode.ErrorWhileSavingDefaultSettings;
            }

            return ExitCode.Ok;
        }
    }
}
