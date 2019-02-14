using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class CreatorSettingsMigrationStorage : IStorage
    {
        private readonly IStorage _baseStorage;
        private readonly int _targetVersion;

        public CreatorSettingsMigrationStorage(IStorage baseStorage, int targetVersion)
        {
            _baseStorage = baseStorage;
            _targetVersion = targetVersion;
        }

        public void ReadData(Data data)
        {
            _baseStorage.ReadData(data);

            var upgrader = new CreatorSettingsUpgrader(data);

            if (upgrader.RequiresUpgrade(_targetVersion))
                upgrader.Upgrade(_targetVersion);
        }

        public void WriteData(Data data)
        {
            _baseStorage.WriteData(data);
        }
    }
}
