using CommandLineParser;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public class HelpCommandExecutor : ICommandExecutor
    {
        private readonly HelpCommand _command;
        private readonly CommandLineParser.CommandLineParser _commandLineParser;

        public HelpCommandExecutor(HelpCommand command, CommandLineParser.CommandLineParser commandLineParser)
        {
            _command = command;
            _commandLineParser = commandLineParser;
        }

        public void InitializeDependencies()
        {
        }

        public CheckResult IsExecutable() => CheckResult.Success();

        public Task<CommandResult> Execute()
        {
            HelpCommand.WriteHelpToConsole(_commandLineParser, _command);
            return Task.FromResult(CommandResult.Success());
        }
    }
}
