using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Events;
using Prism.Events;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands
{
    public class WaitMainShellClosedCommand : IWaitableCommand
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IShellManager _shellManager;

        public WaitMainShellClosedCommand(IEventAggregator eventAggregator, IShellManager shellManager)
        {
            _eventAggregator = eventAggregator;
            _shellManager = shellManager;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (!_shellManager.MainShellIsOpen)
            {
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
                return;
            }
            _eventAggregator.GetEvent<MainShellClosedEvent>().Subscribe(OnWindowClosedAction);
        }

        private void OnWindowClosedAction()
        {
            _eventAggregator.GetEvent<MainShellClosedEvent>().Unsubscribe(OnWindowClosedAction);
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;

#pragma warning restore 67
    }
}
