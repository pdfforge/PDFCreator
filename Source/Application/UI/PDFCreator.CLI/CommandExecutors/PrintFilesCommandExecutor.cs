using pdfforge.PDFCreator.Core.Printing.Printing;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.CLI.Commands;
using pdfforge.PDFCreator.UI.CLI.Helper;
using System.Collections.Generic;
using SystemInterface.IO;
using SystemWrapper.IO;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public class PrintFilesCommandExecutor : PrintFilesCommandExecutorBase
    {
        private readonly PrintFilesCommand _command;
        private IFile _file;
        private IPrintFileHelper _printFileHelper;
        private ISettingsManager _settingsManager;

        public PrintFilesCommandExecutor(PrintFilesCommand command)
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

            InitializeDependencies(new FileWrap(), printFileAssistant, settingsManager);
        }

        protected override IList<string> GetFiles() => _command.Files;

        protected override string GetPrinter() => _command.Printer;

        protected override void PrePrintAction()
        {
        }

        public void InitializeDependencies(IFile file, IPrintFileHelper printFileHelper, ISettingsManager settingsManager)
        {
            _file = file;
            _printFileHelper = printFileHelper;
            _settingsManager = settingsManager;
        }
    }
}
