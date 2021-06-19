namespace pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading
{
    public interface ISettingsUpgrader
    {
        int NumberOfUpgradeMethods();

        void Upgrade(int targetVersion);

        bool RequiresUpgrade(int targetVersion);
    }
}
