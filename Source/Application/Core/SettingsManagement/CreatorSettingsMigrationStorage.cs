using pdfforge.DataStorage;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Utilities;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class CreatorSettingsMigrationStorage : IStorage
    {
        private readonly IStorage _baseStorage;
        private readonly IFontHelper _fontHelper;
        private readonly int _targetVersion;
        private readonly ISettingsBackup _settingsBackup;

        public CreatorSettingsMigrationStorage(IStorage baseStorage, IFontHelper fontHelper, int targetVersion, ISettingsBackup settingsBackup)
        {
            _baseStorage = baseStorage;
            _fontHelper = fontHelper;
            _targetVersion = targetVersion;
            _settingsBackup = settingsBackup;
        }

        public void ReadData(Data data)
        {
            _baseStorage.ReadData(data);

            var upgrader = new CreatorSettingsUpgrader(data, _fontHelper);

            if (upgrader.RequiresUpgrade(_targetVersion))
            {
                var settings = new PdfCreatorSettings();
                settings.ReadValues(data);
                _settingsBackup.SaveSettings(settings);

                upgrader.Upgrade(_targetVersion);
            }
        }

        public void WriteData(Data data)
        {
            _baseStorage.WriteData(data);
        }
    }
}
