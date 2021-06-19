using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper.ActionHelper;
using Prism.Events;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class AddActionCommand : ICommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly IEventAggregator _eventAggregator;
        private readonly EditionHelper _editionHelper;

        public AddActionCommand(IInteractionRequest interactionRequest,
            IEventAggregator eventAggregator, EditionHelper editionHelper)
        {
            _interactionRequest = interactionRequest;
            _eventAggregator = eventAggregator;
            _editionHelper = editionHelper;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        private bool IsSupported(IPresenterActionFacade actionFacade)
        {
            if (_editionHelper.IsFreeEdition)
            {
                return !typeof(IBusinessFeatureAction).IsAssignableFrom(actionFacade.ActionType);
            }

            return true;
        }

        public async void Execute(object parameter)
        {
            var actionFacade = (IPresenterActionFacade)parameter;

            var isSupported = IsSupported(actionFacade);

            if (isSupported)
                actionFacade.AddAction();

            var interaction = new WorkflowEditorOverlayInteraction(actionFacade.Title, actionFacade.OverlayViewName, !isSupported, true);
            interaction = await _interactionRequest.RaiseAsync(interaction);
            if (interaction.Result != WorkflowEditorOverlayResult.Success)
            {
                actionFacade.RemoveAction();
            }

            _eventAggregator.GetEvent<ActionAddedToWorkflowEvent>().Publish();

            if (interaction.Result == WorkflowEditorOverlayResult.Back)
            {
                await _interactionRequest.RaiseAsync(new AddActionOverlayInteraction());
            }
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
