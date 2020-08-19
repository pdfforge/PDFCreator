using pdfforge.PDFCreator.Conversion.Settings;
using System;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface ISettingsBackup
    {
        void SaveSettings(PdfCreatorSettings settings);
    }

    public class SettingsBackup : ISettingsBackup
    {
        private readonly IDataStorageFactory _storageFactory;
        private readonly IDirectory _directory;
        private readonly IAppDataProvider _appDataProvider;

        public SettingsBackup(IDataStorageFactory storageFactory, IDirectory directory, IAppDataProvider appDataProvider)
        {
            _storageFactory = storageFactory;
            _directory = directory;
            _appDataProvider = appDataProvider;
        }

        public void SaveSettings(PdfCreatorSettings settings)
        {
            var path = _appDataProvider.LocalAppDataFolder;

            var backupFile = DateTime.Now.ToString("yyyyMMdd") + "_backup.ini";
            backupFile = PathSafe.Combine(path, backupFile);

            _directory.CreateDirectory(path);

            var storage = _storageFactory.BuildIniStorage(backupFile);
            settings.SaveData(storage);
        }
    }
}
