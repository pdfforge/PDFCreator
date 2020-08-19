using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Conversion.Actions.Actions;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Workflow;
using pdfforge.PDFCreator.Core.SettingsManagement;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using Prism.Events;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor
{
    public class AddActionCommand : ICommand
    {
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly IInteractionRequest _interactionRequest;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICurrentSettings<ApplicationSettings> _settingsProvider;
        private readonly IActionOrderHelper _actionOrderHelper;
        private readonly EditionHelper _editionHelper;

        public AddActionCommand(ISelectedProfileProvider selectedProfileProvider, IInteractionRequest interactionRequest, IEventAggregator eventAggregator,
            ICurrentSettings<ApplicationSettings> settingsProvider, IActionOrderHelper actionOrderHelper, EditionHelper editionHelper)
        {
            _selectedProfileProvider = selectedProfileProvider;
            _interactionRequest = interactionRequest;
            _eventAggregator = eventAggregator;
            _settingsProvider = settingsProvider;
            _actionOrderHelper = actionOrderHelper;
            _editionHelper = editionHelper;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            var actionFacade = (IPresenterActionFacade)parameter;
            var profile = _selectedProfileProvider.SelectedProfile;

            if (!profile.ActionOrder.Exists(x => x == actionFacade.SettingsType.Name))
            {
                if (!_editionHelper.IsFreeEdition || !typeof(IBusinessFeatureAction).IsAssignableFrom(actionFacade.Action))
                {
                    actionFacade.IsEnabled = true;
                    profile.ActionOrder.Add(actionFacade.SettingsType.Name);
                }

                _actionOrderHelper.EnsureEncryptionAndSignatureOrder(profile);

                if (profile.EnableWorkflowEditor)
                {
                    var result = await _interactionRequest.RaiseAsync(new WorkflowEditorOverlayInteraction(false, actionFacade.Translation, actionFacade.OverlayView));
                    if (!result.Success)
                    {
                        actionFacade.IsEnabled = false;
                        _selectedProfileProvider.SelectedProfile.ActionOrder.RemoveAll(x => x == actionFacade.SettingsType.Name);

                        if ("CoverPage" == actionFacade.SettingsType.Name)
                            _selectedProfileProvider.SelectedProfile.CoverPage.File = string.Empty;
                    }
                }
                else
                {
                    _actionOrderHelper.ForceDefaultOrder(profile);
                }

                _eventAggregator.GetEvent<ActionAddedToWorkflowEvent>().Publish();
            }
        }

#pragma warning disable CS0067

        public event EventHandler CanExecuteChanged;

#pragma warning restore CS0067
    }
}
