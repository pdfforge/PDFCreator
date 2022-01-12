using pdfforge.PDFCreator.Core.DirectConversion;
using pdfforge.PDFCreator.UI.CLI.Commands;
using pdfforge.PDFCreator.UI.CLI.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public class MergeAndProcessFilesCommandExecutor : ICommandExecutor
    {
        private readonly MergeAndProcessFilesCommand _command;
        private IDirectConversionInfFileHelper _directConversionInfFileHelper;
        private IDirectConversionHelper _directConversionHelper;

        public MergeAndProcessFilesCommandExecutor(MergeAndProcessFilesCommand command)
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
            var unsupportedFiles = _command.Files.Where(f => !_directConversionHelper.CanConvertDirectly(f)).ToList();

            if (unsupportedFiles.Any())
                return CheckResult.Error($"The files cannot be processed directly, please use the PrintFiles command instead:\n" + string.Join("\n", unsupportedFiles));

            return CheckResult.Success();
        }

        public Task<CommandResult> Execute()
        {
            var parameters = new AppStartParameters()
            {
                Profile = _command.Profile,
                OutputFile = _command.OutputFile
            };

            var infFile = _directConversionInfFileHelper.TransformToInfFileWithMerge(_command.Files, parameters);

            Console.WriteLine("Created job file {0}", infFile);

            var newJobCommand = new NewPrintJobCommand
            {
                InfFilePath = infFile
            };

            var newJobExecutor = new NewPrintJobExecutor(newJobCommand);
            newJobExecutor.InitializeDependencies();

            return newJobExecutor.Execute();
        }
    }
}
