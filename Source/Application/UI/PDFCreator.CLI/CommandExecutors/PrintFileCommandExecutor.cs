using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.CLI.Commands;
using pdfforge.PDFCreator.UI.CLI.Helper;
using System.Collections.Generic;
using SystemInterface.IO;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public class PrintFileCommandExecutor : PrintFilesCommandExecutorBase
    {
        private readonly PrintFileCommand _command;
        private IFile _file;
        private IPrintFileHelper _printFileHelper;
        private ISettingsManager _settingsManager;
        private IStoredParametersManager _storedParametersManager;

        public PrintFileCommandExecutor(PrintFileCommand command)
        {
            _command = command;
        }

        protected override IFile File => _file;
        protected override IPrintFileHelper PrintFileHelper => _printFileHelper;
        protected override ISettingsManager SettingsManager => _settingsManager;

        public override void InitializeDependencies()
        {
            var container = BootstrapperHelper.GetConfiguredContainer();

            var settingsManager = container.GetInstance<ISettingsManager>();
            var printFileAssistant = container.GetInstance<IPrintFileHelper>();
            var storedParametersManager = container.GetInstance<IStoredParametersManager>();

            InitializeDependencies(new FileWrap(), printFileAssistant, settingsManager, storedParametersManager);
        }

        public void InitializeDependencies(IFile file, IPrintFileHelper printFileHelper, ISettingsManager settingsManager, IStoredParametersManager storedParametersManager)
        {
            _file = file;
            _printFileHelper = printFileHelper;
            _settingsManager = settingsManager;
            _storedParametersManager = storedParametersManager;
        }

        protected override IList<string> GetFiles() => new List<string> { _command.File };

        protected override string GetPrinter() => _command.Printer;

        protected override void PrePrintAction()
        {
            var settingsProvider = _settingsManager.GetSettingsProvider();
            settingsProvider.Settings.CreatorAppSettings.AskSwitchDefaultPrinter = !_command.AllowSwitchDefaultPrinter;

            // Ignore Profile if Printer is set.
            // After the printing we can't determine if the printer was selected by the user or was the primary printer
            var profile = "";
            if (string.IsNullOrWhiteSpace(_command.Printer))
                profile = _command.Profile;

            _storedParametersManager.SaveParameterSettings(_command.OutputFile, profile, _command.File);
        }
    }
}
