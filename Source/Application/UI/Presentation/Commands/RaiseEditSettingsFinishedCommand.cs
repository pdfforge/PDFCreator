using pdfforge.PDFCreator.UI.Presentation.Events;
using Prism.Events;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class RaiseEditSettingsFinishedEventCommand : ICommand
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ISelectedProfileProvider _selectedProfileProvider;

        public RaiseEditSettingsFinishedEventCommand(IEventAggregator eventAggregator, ISelectedProfileProvider selectedProfileProvider)
        {
            _eventAggregator = eventAggregator;
            _selectedProfileProvider = selectedProfileProvider;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            _eventAggregator.GetEvent<EditSettingsFinishedEvent>().Publish(_selectedProfileProvider.SelectedProfile);
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }
}
