using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using System;
using System.Security;
using System.Threading.Tasks;
using SystemInterface.Microsoft.Win32;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;

namespace pdfforge.PDFCreator.Core.Startup.AppStarts
{
    public class StoreLicenseForAllUsersStart : AppStartBase
    {
        private readonly IRegistry _registry;
        private string _registryPath;

        public string LicenseServerCode { get; set; }
        public string LicenseKey { get; set; }

        public StoreLicenseForAllUsersStart(ICheckAllStartupConditions checkAllStartupConditions, IRegistry registry, IInstallationPathProvider installationPathProvider)
            : base(checkAllStartupConditions)
        {
            _registry = registry;
            _registryPath = installationPathProvider.ApplicationRegistryPath;
            SkipStartupConditionCheck = true;
        }

        public override Task<ExitCode> Run()
        {
            if (string.IsNullOrWhiteSpace(LicenseServerCode))
                return Task.FromResult(ExitCode.MissingActivation);

            var regPath = "HKEY_LOCAL_MACHINE\\" + _registryPath;

            try
            {
                _registry.SetValue(regPath, "License", LicenseKey);
                _registry.SetValue(regPath, "LSA", LicenseServerCode);
            }
            catch (SecurityException)
            {
                return Task.FromResult(ExitCode.NoAccessPrivileges);
            }
            catch (Exception)
            {
                return Task.FromResult(ExitCode.Unknown);
            }

            return Task.FromResult(ExitCode.Ok);
        }
    }
}
