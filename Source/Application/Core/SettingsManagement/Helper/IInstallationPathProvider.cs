using System;
using Microsoft.Win32;

namespace pdfforge.PDFCreator.Core.SettingsManagement.Helper
{
    public interface IInstallationPathProvider
    {
        /// <summary>
        ///     The registry path where the settings are stored, without the registry hive.
        ///     i.e. "Software\pdfforge\PDFCreator\Settings"
        /// </summary>
        string SettingsRegistryPath { get; }

        /// <summary>
        ///     The registry path where the application data are stored, without the registry hive.
        ///     i.e. "Software\pdfforge\PDFCreator"
        /// </summary>
        string ApplicationRegistryPath { get; }

        string RegistryHive { get; }

        /// <summary>
        ///     The GUID with curly braces, i.e. {00000000-0000-0000-0000-000000000000}
        /// </summary>
        string ApplicationGuid { get; }
    }

    public class InstallationPathProvider : IInstallationPathProvider
    {
        public InstallationPathProvider(string applicationRegistryPath, string settingsRegistryPath,
            string applicationGuid, RegistryHive registryHive)
        {
            SettingsRegistryPath = settingsRegistryPath;
            ApplicationGuid = applicationGuid;
            ApplicationRegistryPath = applicationRegistryPath;
            RegistryHive = GetHiveString(registryHive);
        }

        private string GetHiveString(RegistryHive registryHive)
        {
            switch (registryHive)
            {
                case Microsoft.Win32.RegistryHive.CurrentUser: return "HKEY_CURRENT_USER";
                case Microsoft.Win32.RegistryHive.LocalMachine: return "HKEY_LOCAL_MACHINE";
            }

            throw new ArgumentOutOfRangeException(nameof(registryHive), $"The registry hive {registryHive} is not supported!");
        }

        public string SettingsRegistryPath { get; }
        public string ApplicationRegistryPath { get; }
        public string ApplicationGuid { get; }
        public string RegistryHive { get; }
    }
}
