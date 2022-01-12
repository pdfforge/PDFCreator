using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.UI.CLI.Commands;
using pdfforge.PDFCreator.UI.CLI.Helper;
using System;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public class ProcessFileCommandExecutor : ICommandExecutor
    {
        private readonly ProcessFileCommand _command;
        private IDirectConversionInfFileHelper _directConversionInfFileHelper;
        private IDirectConversionHelper _directConversionHelper;

        public ProcessFileCommandExecutor(ProcessFileCommand command)
        {
            _command = command;
        }

        public void InitializeDependencies()
        {
            var container = BootstrapperHelper.GetConfiguredContainer();

            var directConversionHelper = container.GetInstance<IDirectConversionHelper>();
            var directConversionInfFileHelper = container.GetInstance<IDirectConversionInfFileHelper>();

            InitializeDependencies(directConversionHelper, directConversionInfFileHelper);
        }

        public void InitializeDependencies(IDirectConversionHelper directConversionHelper, IDirectConversionInfFileHelper directConversionInfFileHelper)
        {
            _directConversionHelper = directConversionHelper;
            _directConversionInfFileHelper = directConversionInfFileHelper;
        }

        public CheckResult IsExecutable()
        {
            if (!_directConversionHelper.CanConvertDirectly(_command.File))
                return CheckResult.Error($"The file '{_command.File}' cannot be processed directly, please use the PrintFiles command instead.");

            return CheckResult.Success();
        }

        public Task<CommandResult> Execute()
        {
            var parameters = new AppStartParameters()
            {
                Profile = _command.Profile,
                OutputFile = _command.OutputFile
            };

            Console.WriteLine("Processing {0}", _command.File);

            var infFile = _directConversionInfFileHelper.TransformToInfFile(_command.File, parameters);

            Console.WriteLine("Created job file {0}", infFile);

            var newJobCommand = new NewPrintJobCommand()
            {
                InfFilePath = infFile
            };

            var newJobExecutor = new NewPrintJobExecutor(newJobCommand);
            newJobExecutor.InitializeDependencies();

            return newJobExecutor.Execute();
        }
    }
}
