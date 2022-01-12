using pdfforge.PDFCreator.Core.StartupInterface;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public class CommandResult
    {
        public CommandResult(bool success, int exitCode, string message)
        {
            IsSuccess = success;
            ExitCode = exitCode;
            Message = message;
        }

        public bool IsSuccess { get; }
        public int ExitCode { get; }
        public string Message { get; }

        public static CommandResult Success() => new CommandResult(true, 0, "");

        public static CommandResult Error(int exitCode, string message) => new CommandResult(false, exitCode, message);

        public static CommandResult Error(ExitCode exitCode, string message) => new CommandResult(false, (int)exitCode, message);
    }
}
