using pdfforge.PDFCreator.UI.Presentation.Events;
using Prism.Events;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class RaiseEditSettingsFinishedEventCommand : ICommand
    {
        private readonly IEventAggregator _eventAggregator;

        public RaiseEditSettingsFinishedEventCommand(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _eventAggregator.GetEvent<EditSettingsFinishedEvent>().Publish();
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }
}
