using pdfforge.DataStorage.Storage;
using System;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class MigrationStorageFactory : IMigrationStorageFactory
    {
        private readonly Func<IStorage, int, ISettingsBackup, IStorage> _createFunc;

        public MigrationStorageFactory(Func<IStorage, int, ISettingsBackup, IStorage> createFunc)
        {
            _createFunc = createFunc;
        }

        public IStorage GetMigrationStorage(IStorage baseStorage, int targetVersion, ISettingsBackup settingsBackup)
        {
            if (baseStorage == null)
                return null;

            return _createFunc.Invoke(baseStorage, targetVersion, settingsBackup);
        }
    }

    public interface IMigrationStorageFactory
    {
        IStorage GetMigrationStorage(IStorage baseStorage, int targetVersion, ISettingsBackup settingsBackup);
    }
}
