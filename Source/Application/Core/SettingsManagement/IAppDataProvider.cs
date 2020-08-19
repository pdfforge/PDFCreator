namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public interface IAppDataProvider
    {
        string LocalAppDataFolder { get; }

        string RoamingAppDataFolder { get; }
    }
}
