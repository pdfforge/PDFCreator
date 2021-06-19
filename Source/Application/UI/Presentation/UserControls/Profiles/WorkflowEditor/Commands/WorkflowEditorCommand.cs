using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.Macros;
using Prism.Events;
using System;
using System.Threading.Tasks;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.WorkflowEditor.Commands
{
    public class WorkflowEditorCommand : WaitableAsyncCommandBase
    {
        private readonly ISelectedProfileProvider _selectedProfileProvider;
        private readonly IInteractionRequest _interactionRequest;
        private readonly ITranslationFactory _translationFactory;
        private readonly IEventAggregator _eventAggregator;

        private string _view;
        private Func<SaveOutputFormatMetadataTranslation, string> _getTitle;

        public WorkflowEditorCommand(ISelectedProfileProvider selectedProfileProvider, IInteractionRequest interactionRequest, ITranslationFactory translationFactory, IEventAggregator eventAggregator)
        {
            _selectedProfileProvider = selectedProfileProvider;
            _interactionRequest = interactionRequest;
            _translationFactory = translationFactory;
            _eventAggregator = eventAggregator;
        }

        public void Initialize(string view, Func<SaveOutputFormatMetadataTranslation, string> getTitle)
        {
            _view = view;
            _getTitle = getTitle;
        }

        public override bool QueryCanExecute(object parameter) => true;

        public override async Task<MacroCommandIsDoneEventArgs> ExecuteWaitableAsync(object parameter)
        {
            var settingsCopy = _selectedProfileProvider.SelectedProfile.Copy();

            var translation = _translationFactory.CreateTranslation<SaveOutputFormatMetadataTranslation>();
            var title = _getTitle(translation);

            var interaction = new WorkflowEditorOverlayInteraction(title, _view, false, false);

            await _interactionRequest.RaiseAsync(interaction);
            if (interaction.Result != WorkflowEditorOverlayResult.Success && !settingsCopy.Equals(_selectedProfileProvider.SelectedProfile))
            {
                _selectedProfileProvider.SelectedProfile.ReplaceWith(settingsCopy);
            }

            _eventAggregator.GetEvent<WorkflowSettingsChanged>().Publish();

            var status = interaction.Result == WorkflowEditorOverlayResult.Success ? ResponseStatus.Success : ResponseStatus.Cancel;

            return new MacroCommandIsDoneEventArgs(status);
        }
    }
}
