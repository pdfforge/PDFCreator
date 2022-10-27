namespace pdfforge.PDFCreator.Core.SettingsManagementInterface
{
    public interface IInstallationPathProvider
    {
        /// <summary>
        ///     The registry path where the settings are stored, without the registry hive.
        ///     e.g. "Software\pdfforge\PDFCreator\Settings"
        /// </summary>
        string SettingsRegistryPath { get; }

        /// <summary>
        ///     The registry path where the application data are stored, without the registry hive.
        ///     e.g. "Software\pdfforge\PDFCreator"
        /// </summary>
        string ApplicationRegistryPath { get; }

        string RegistryHive { get; }

        /// <summary>
        ///     The GUID with curly braces, i.e. {00000000-0000-0000-0000-000000000000}
        /// </summary>
        string ApplicationGuid { get; }
    }
}
