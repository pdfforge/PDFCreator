using pdfforge.PDFCreator.Core.Services.Macros;
using System.Windows.Input;

namespace pdfforge.PDFCreator.Core.Services
{
    public interface ICommandLocator
    {
        ICommand GetCommand<T>() where T : class, ICommand;

        ICommand GetInitializedCommand<TCommand, TParameter>(TParameter parameter) where TCommand : class, IInitializedCommand<TParameter>;

        IMacroCommandBuilder CreateMacroCommand();
    }
}