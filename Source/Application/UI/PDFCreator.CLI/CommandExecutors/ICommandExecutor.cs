using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public interface ICommandExecutor
    {
        /// <summary>
        /// Create and initialize the dependencies for this executor. To minimize runtime requirements, we initialize the dependencies only for the command we actually want to execute.
        /// </summary>
        void InitializeDependencies();

        /// <summary>
        /// Check if the command is executable, e.g. some dependencies are missing.
        /// </summary>
        /// <returns>A successful result, if the command can be executed</returns>
        CheckResult IsExecutable();

        /// <summary>
        /// Execute the actual command
        /// </summary>
        /// <returns>The exit code for the application indicating the success</returns>
        Task<CommandResult> Execute();
    }
}
