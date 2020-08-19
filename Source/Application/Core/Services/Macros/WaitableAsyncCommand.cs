using pdfforge.Obsidian;
using System;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Services.Macros
{
    public class WaitableAsyncCommand : AsyncCommandBase, IWaitableCommand
    {
        private readonly Func<object, Task<MacroCommandIsDoneEventArgs>> _command;
        private readonly Predicate<object> _canExecute;

        public WaitableAsyncCommand(Func<object, Task<MacroCommandIsDoneEventArgs>> command, Predicate<object> canExecute = null)
        {
            this._command = command;
            this._canExecute = canExecute;
        }

        public NotifyTaskCompletion<MacroCommandIsDoneEventArgs> Execution { get; private set; }

        public override bool CanExecute(object parameter)
        {
            NotifyTaskCompletion<MacroCommandIsDoneEventArgs> execution = this.Execution;
            if (execution != null && !execution.IsCompleted)
                return false;
            if (this._canExecute != null)
                return this._canExecute(parameter);
            return true;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            this.Execution = new NotifyTaskCompletion<MacroCommandIsDoneEventArgs>(this._command(parameter));
            this.RaisePropertyChanged("Execution");
            this.RaisePropertyChanged("IsExecuting");
            this.RaiseCanExecuteChanged();
            var macroCommandIsDoneEventArgs = await this.Execution.Task;
            IsDone?.Invoke(this, macroCommandIsDoneEventArgs);
            this.RaiseCanExecuteChanged();
            this.RaisePropertyChanged("IsExecuting");
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
