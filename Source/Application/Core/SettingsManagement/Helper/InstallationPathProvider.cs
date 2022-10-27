using System;
using Microsoft.Win32;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;

namespace pdfforge.PDFCreator.Core.SettingsManagement.Helper
{
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

    public static class InstallationPathProviders
    {
        private static string _pdfcreatorRegPath = @"Software\pdfforge\PDFCreator";
        private static string _pdfcreatorServerRegPath = @"Software\pdfforge\PDFCreator Server";
        private static string _pdfcreatorProductId = "{0001B4FD-9EA3-4D90-A79E-FD14BA3AB01D}";

        public static InstallationPathProvider PDFCreatorProvider => new InstallationPathProvider(_pdfcreatorRegPath, _pdfcreatorRegPath + @"\Settings", _pdfcreatorProductId, RegistryHive.CurrentUser);
        public static InstallationPathProvider PDFCreatorServerProvider => new InstallationPathProvider(_pdfcreatorServerRegPath, _pdfcreatorServerRegPath + @"\Settings", _pdfcreatorProductId, RegistryHive.LocalMachine);
    }
}
