using pdfforge.Communication;
using pdfforge.PDFCreator.UI.CLI.Commands;
using pdfforge.PDFCreator.UI.CLI.Helper;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagementInterface;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    internal class NewPrintJobExecutor : ICommandExecutor
    {
        private readonly NewPrintJobCommand _newPrintJobCommand;
        private readonly string _pipeName = "PDFCreator-" + Process.GetCurrentProcess().SessionId;
        private ApplicationLauncher _launcher;

        public NewPrintJobExecutor(NewPrintJobCommand newPrintJobCommand)
        {
            _newPrintJobCommand = newPrintJobCommand;
        }

        public void InitializeDependencies()
        {
            _launcher = new ApplicationLauncher("PDFCreator.exe", InstallationPathProviders.PDFCreatorProvider);
        }

        public CheckResult IsExecutable()
        {
            if (!_launcher.CanLaunchApplication())
                return CheckResult.Error("Could not find PDFCreator.exe!");

            return CheckResult.Success();
        }

        public Task<CommandResult> Execute()
        {
            var pipe = new PipeClient(_pipeName);
            var pipeServer = new PipeServer(_pipeName, _pipeName);

            var timeout = TimeSpan.FromSeconds(10);

            try
            {
                if (!File.Exists(_newPrintJobCommand.InfFilePath))
                    return Task.FromResult(CommandResult.Error(1, "The INF file does not exist!"));

                if (pipeServer.IsServerRunning())
                {
                    Console.WriteLine("Pipe server is running");
                    pipe.SendMessage("NewJob|" + _newPrintJobCommand.InfFilePath, (int)timeout.TotalMilliseconds);
                    Console.WriteLine("Pipe message has been sent");

                    return Task.FromResult(CommandResult.Success());
                }
            }
            catch
            {
                // Ignore exception and launch PDFCreator
            }

            Console.WriteLine("Launching PDFCreator");

            try
            {
                var _ = _launcher.LaunchApplication($"/InfoDataFile={_newPrintJobCommand.InfFilePath}");
            }
            catch
            {
                return Task.FromResult(CommandResult.Error(1, "Could not start PDFCreator!"));
            }

            return Task.FromResult(CommandResult.Success());
        }
    }
}
