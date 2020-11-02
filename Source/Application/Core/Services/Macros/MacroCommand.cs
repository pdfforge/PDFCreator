using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace pdfforge.PDFCreator.Core.Services.Macros
{
    public class MacroCommand : IMacroCommand
    {
        protected List<ICommand> CommandList;
        private int _runIndex = 0;
        private object _parameter;
        private BooleanMacroResult _result;
        private Action<BooleanMacroResult> _callback = null;

        public event EventHandler MacroIsDone;

        public ICommand GetCommand(int index)
        {
            if (index > CommandList.Count)
                return null;
            return CommandList[index];
        }

        public MacroCommand(List<ICommand> commandList)
        {
            CommandList = commandList;

            foreach (var command in commandList)
            {
                command.CanExecuteChanged += (sender, args) => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ExecuteWithAsyncResult(object parameter, Action<BooleanMacroResult> callback = null)
        {
            _callback = callback;
            _result = new BooleanMacroResult(false);
            Execute(parameter);
        }

        public BooleanMacroResult ExecuteWithResult(object parameter)
        {
            _result = new BooleanMacroResult(false);
            Execute(parameter);
            return _result;
        }

        private void Next(object sender, MacroCommandIsDoneEventArgs e)
        {
            if (sender is IWaitableCommand previousCommand)
                previousCommand.IsDone -= Next;

            if (e.ResponseStatus == ResponseStatus.Cancel || e.ResponseStatus == ResponseStatus.Error)
            {
                InvokeAllDone(false);
                return;
            }

            _runIndex++;

            if (e.ResponseStatus == ResponseStatus.Skip)
            {
                Next(sender, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
            }

            if (CommandList.Count > _runIndex)
            {
                var command = CommandList.ElementAt(_runIndex);

                switch (command)
                {
                    case IWaitableCommand waitableCommand:
                        waitableCommand.IsDone += Next;
                        waitableCommand.Execute(_parameter);
                        break;

                    default:
                        command.Execute(_parameter);
                        Next(command, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
                        break;
                }
            }
            else
            {
                InvokeAllDone(true);
            }
        }

        private void InvokeAllDone(bool success)
        {
            _result?.SetResult(success);
            MacroIsDone?.Invoke(this, new MacroAreDoneEventArgs(_result));
            _callback?.Invoke(_result);
        }

        public bool CanExecute(object parameter)
        {
            foreach (var command in CommandList)
            {
                if (!command.CanExecute(parameter))
                    return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            _parameter = parameter;
            _runIndex = -1;

            Next(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        public async Task<ResponseStatus> ExecuteAsync(object parameter)
        {
            ResponseStatus status = ResponseStatus.Success;

            foreach (var command in CommandList)
            {
                if (status == ResponseStatus.Skip)
                {
                    status = ResponseStatus.Success;
                    continue;
                }

                if (command is IWaitableCommand waitableCommand)
                {
                    var taskCompletionSource = new TaskCompletionSource<ResponseStatus>();
                    void IsDoneHandler(object sender, MacroCommandIsDoneEventArgs args) => taskCompletionSource.SetResult(args.ResponseStatus);

                    waitableCommand.IsDone += IsDoneHandler;

                    waitableCommand.Execute(parameter);
                    status = await taskCompletionSource.Task;

                    waitableCommand.IsDone -= IsDoneHandler;
                }
                else
                {
                    command.Execute(parameter);
                }

                if (status == ResponseStatus.Cancel || status == ResponseStatus.Error)
                {
                    break;
                }
            }

            InvokeAllDone(true);
            return status;
        }

        public void MountView()
        {
            foreach (var command in CommandList)
            {
                if (command is IMountable mountable)
                    mountable.MountView();
            }
        }

        public void UnmountView()
        {
            foreach (var command in CommandList)
            {
                if (command is IMountable mountable)
                    mountable.UnmountView();
            }
        }

        // has to be implemented because of ICommand
#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }

    public enum ResponseStatus
    {
        Success,
        Cancel,
        Skip,
        Error
    }
}
