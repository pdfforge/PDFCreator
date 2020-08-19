using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Presentation.Events;
using Prism.Events;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.ProfileCommands
{
    public class WaitProfileModificationCommand : IWaitableCommand
    {
        private readonly IEventAggregator _eventAggregator;

        public WaitProfileModificationCommand(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _eventAggregator.GetEvent<EditSettingsFinishedEvent>().Subscribe(EditSettingsFinishedWindowClosedAction);
            _eventAggregator.GetEvent<MainWindowClosedEvent>().Subscribe(OnWindowClosedAction);
        }

        private void OnWindowClosedAction()
        {
            _eventAggregator.GetEvent<EditSettingsFinishedEvent>().Unsubscribe(EditSettingsFinishedWindowClosedAction);
            _eventAggregator.GetEvent<MainWindowClosedEvent>().Unsubscribe(OnWindowClosedAction);
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        private void EditSettingsFinishedWindowClosedAction(ConversionProfile p)
        {
            OnWindowClosedAction();
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;

#pragma warning restore 67
    }
}
