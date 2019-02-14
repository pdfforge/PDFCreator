namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public class SettingsUpgradeHelper
    {
        private readonly int _targetVersion;

        public SettingsUpgradeHelper(int targetVersion)
        {
            _targetVersion = targetVersion;
        }

        public void UpgradeSettings<TSettingsUpgrader>(TSettingsUpgrader upgrader) where TSettingsUpgrader : ISettingsUpgrader
        {
            if (upgrader != null && upgrader.RequiresUpgrade(_targetVersion))
                upgrader.Upgrade(_targetVersion);
        }
    }
}
