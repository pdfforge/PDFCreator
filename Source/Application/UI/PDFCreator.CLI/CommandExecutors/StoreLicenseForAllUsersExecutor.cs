using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using pdfforge.PDFCreator.UI.CLI.Commands;
using System;
using System.Security;
using System.Threading.Tasks;
using SystemInterface.Microsoft.Win32;
using SystemWrapper.Microsoft.Win32;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public class StoreLicenseForAllUsersExecutor : ICommandExecutor
    {
        private readonly StoreLicenseForAllUsersCommand _command;
        private IRegistry _registry;
        private IInstallationPathProvider _installationPathProvider;

        public StoreLicenseForAllUsersExecutor(StoreLicenseForAllUsersCommand command)
        {
            _command = command;
        }

        public void InitializeDependencies()
        {
            InitializeDependencies(new RegistryWrap(), InstallationPathProviders.PDFCreatorProvider);
        }

        public void InitializeDependencies(IRegistry registry, IInstallationPathProvider installationPathProvider)
        {
            _registry = registry;
            _installationPathProvider = installationPathProvider;
        }

        public CheckResult IsExecutable() => CheckResult.Success();

        public Task<CommandResult> Execute()
        {
            var regPath = "HKEY_LOCAL_MACHINE\\" + _installationPathProvider.ApplicationRegistryPath;

            if (string.IsNullOrWhiteSpace(_command.LicenseServerCode))
            {
                return Task.FromResult(CommandResult.Error(2, "No license server code was provided!"));
            }

            try
            {
                var regKey = _registry.LocalMachine.OpenSubKey(_installationPathProvider.ApplicationRegistryPath, true);
                regKey.SetValue("License", _command.LicenseKey);
                regKey.SetValue("LSA", _command.LicenseServerCode);
                regKey.DeleteValue("LicenseCheckDeferredAt", false);
            }
            catch (Exception e)
            {
                if (e is UnauthorizedAccessException || e is SecurityException)
                    return Task.FromResult(CommandResult.Error(2, "Insufficient access rights to write to " + regPath));
                return Task.FromResult(CommandResult.Error(2, "An unexpected error has occurred"));
            }

            return Task.FromResult(CommandResult.Success());
        }
    }
}
