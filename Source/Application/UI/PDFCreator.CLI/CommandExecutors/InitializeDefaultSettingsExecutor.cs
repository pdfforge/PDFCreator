using Microsoft.Win32;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.CLI.Commands;
using pdfforge.PDFCreator.UI.CLI.Helper;
using System;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public class InitializeDefaultSettingsExecutor : ICommandExecutor
    {
        private readonly InitializeDefaultSettingsCommand _command;
        private IIniSettingsLoader _iniSettingsLoader;
        private ISettingsProvider _settingsProvider;
        private IInstallationPathProvider _pathProvider;
        private IDataStorageFactory _storageFactory;
        private IFile _file;

        public InitializeDefaultSettingsExecutor(InitializeDefaultSettingsCommand command)
        {
            _command = command;
        }

        public void InitializeDependencies()
        {
            var container = BootstrapperHelper.GetConfiguredContainer();

            InitializeDependencies(container.GetInstance<IFile>(), container.GetInstance<IIniSettingsLoader>(), container.GetInstance<ISettingsProvider>(), container.GetInstance<IInstallationPathProvider>(), container.GetInstance<IDataStorageFactory>());
        }

        public void InitializeDependencies(IFile file, IIniSettingsLoader iniSettingsLoader, ISettingsProvider settingsProvider, IInstallationPathProvider pathProvider, IDataStorageFactory storageFactory)
        {
            _file = file;
            _iniSettingsLoader = iniSettingsLoader;
            _settingsProvider = settingsProvider;
            _pathProvider = pathProvider;
            _storageFactory = storageFactory;
        }

        public CheckResult IsExecutable()
        {
            return CheckResult.Success();
        }

        public Task<CommandResult> Execute()
        {
            if (!_file.Exists(_command.SettingsFile))
                return Task.FromResult(CommandResult.Error(ExitCode.InvalidSettingsFile, "The settings file does not exist!"));

            var settings = _iniSettingsLoader.LoadIniSettings(_command.SettingsFile);
            if (settings == null)
                return Task.FromResult(CommandResult.Error(ExitCode.InvalidSettingsFile, "The settings file does not exist!"));

            if (!_settingsProvider.CheckValidSettings(settings as PdfCreatorSettings))
                return Task.FromResult(CommandResult.Error(ExitCode.InvalidSettingsInGivenFile, "The file is not a valid settings file!"));

            try
            {
                var storage = _storageFactory.BuildRegistryStorage(RegistryHive.Users, ".Default\\" + _pathProvider.SettingsRegistryPath);
                var success = settings.SaveData(storage);
                if (!success)
                    return Task.FromResult(CommandResult.Error(ExitCode.ErrorWhileSavingDefaultSettings, "You do not have sufficient permissions to write to HKEY_USERS\\.Default"));
            }
            catch (Exception ex)
            {
                return Task.FromResult(CommandResult.Error(ExitCode.ErrorWhileSavingDefaultSettings, "There was an error while saving the settings: " + ex.Message));
            }

            return Task.FromResult(CommandResult.Success());
        }
    }
}
