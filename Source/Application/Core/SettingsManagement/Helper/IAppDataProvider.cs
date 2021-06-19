namespace pdfforge.PDFCreator.Core.SettingsManagement.Helper
{
    public interface IAppDataProvider
    {
        string LocalAppDataFolder { get; }

        string RoamingAppDataFolder { get; }
    }
}
