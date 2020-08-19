using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pdfforge.PDFCreator.Core.Services.Macros
{
    public interface IMacroCommand : ICommand, IMountable
    {
        event EventHandler MacroIsDone;

        ICommand GetCommand(int index);

        Task<ResponseStatus> ExecuteAsync(object parameter);

        BooleanMacroResult ExecuteWithResult(object parameter);
    }
}
