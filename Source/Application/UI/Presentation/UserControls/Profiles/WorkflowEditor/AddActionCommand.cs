using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class AddActionCommand : ICommand
    {
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly IInteractionRequest _interactionRequest;
        private readonly IEventAggregator _eventAggregator;
        private readonly IActionOrderHelper _actionOrderHelper;
        private readonly EditionHelper _editionHelper;

        public AddActionCommand(ISelectedProfileProvider selectedProfileProvider, IInteractionRequest interactionRequest, IEventAggregator eventAggregator,
            IActionOrderHelper actionOrderHelper, EditionHelper editionHelper)
        {
            _selectedProfileProvider = selectedProfileProvider;
            _interactionRequest = interactionRequest;
            _eventAggregator = eventAggregator;
            _actionOrderHelper = actionOrderHelper;
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
                return !typeof(IBusinessFeatureAction).IsAssignableFrom(actionFacade.Action);
            }

            return true;
        }

        public async void Execute(object parameter)
        {
            var actionFacade = (IPresenterActionFacade)parameter;
            var profile = _selectedProfileProvider.SelectedProfile;

            if (!profile.ActionOrder.Exists(x => x == actionFacade.SettingsType.Name))
            {
                var isDisabled = false;
                if (IsSupported(actionFacade))
                {
                    actionFacade.IsEnabled = true;
                    profile.ActionOrder.Add(actionFacade.SettingsType.Name);
                }
                else
                {
                    isDisabled = true;
                }

                _actionOrderHelper.EnsureEncryptionAndSignatureOrder(profile);

                var interaction = await _interactionRequest.RaiseAsync(new WorkflowEditorOverlayInteraction(actionFacade.Translation, actionFacade.OverlayView, isDisabled, true));
                if (interaction.Result != WorkflowEditorOverlayResult.Success)
                {
                    actionFacade.IsEnabled = false;
                    _selectedProfileProvider.SelectedProfile.ActionOrder.RemoveAll(x => x == actionFacade.SettingsType.Name);

                    if ("CoverPage" == actionFacade.SettingsType.Name)
                        _selectedProfileProvider.SelectedProfile.CoverPage.Files = new List<string>();
                }

                _eventAggregator.GetEvent<ActionAddedToWorkflowEvent>().Publish();

                if (interaction.Result == WorkflowEditorOverlayResult.Back)
                {
                    await _interactionRequest.RaiseAsync(new AddActionOverlayInteraction());
                }
            }
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
