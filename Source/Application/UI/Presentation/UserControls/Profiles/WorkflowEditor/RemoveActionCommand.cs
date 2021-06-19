using pdfforge.PDFCreator.UI.Presentation.Helper.ActionHelper;
using Prism.Events;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class RemoveActionCommand : ICommand
    {
        private readonly IEventAggregator _eventAggregator;

        public RemoveActionCommand(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var facade = (IPresenterActionFacade)parameter;
            facade.RemoveAction();
            _eventAggregator.GetEvent<ActionRemovedFromWorkflowEvent>().Publish();
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
