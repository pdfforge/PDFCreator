using System;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.StartupInterface;
using pdfforge.PDFCreator.UI.CLI.Helper;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public class InitializeSettingsExecutor : ICommandExecutor
    {
        private ISettingsManager _settingsManager;

        public void InitializeDependencies()
        {
            var container = BootstrapperHelper.GetConfiguredContainer();
            InitializeDependencies(container.GetInstance<ISettingsManager>());
        }

        public void InitializeDependencies(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public CheckResult IsExecutable()
        {
            return CheckResult.Success();
        }

        public Task<CommandResult> Execute()
        {
            try
            {
                _settingsManager.LoadAllSettings();
                _settingsManager.SaveCurrentSettings();
            }
            catch (Exception ex)
            {
                return Task.FromResult(CommandResult.Error(ExitCode.Unknown, "There was an error while initializing the settings: " + ex.Message));
            }

            return Task.FromResult(CommandResult.Success());
        }
    }
}
