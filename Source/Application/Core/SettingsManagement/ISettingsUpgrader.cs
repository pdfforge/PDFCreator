namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface ISettingsUpgrader
    {
        int NumberOfUpgradeMethods();

        void Upgrade(int targetVersion);

        bool RequiresUpgrade(int targetVersion);
    }
}
