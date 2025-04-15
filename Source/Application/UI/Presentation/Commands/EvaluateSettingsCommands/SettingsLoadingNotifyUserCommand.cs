using pdfforge.PDFCreator.Core.Services.Macros;
using System;
using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Utilities.Messages;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using Prism.Events;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands
{
    internal class SettingsLoadingNotifyUserCommand : TranslatableCommandBase<EvaluateSettingsAndNotifyUserTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;
        private readonly IEventAggregator _eventAggregator;

        public SettingsLoadingNotifyUserCommand(ITranslationUpdater translationUpdater, 
            IInteractionRequest interactionRequest,
            IEventAggregator eventAggregator) 
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
            _eventAggregator = eventAggregator;
        }


        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if(parameter is true)
            {
                var title = Translation.SettingsAreLoadingTitle;
                var text = Translation.ApplicationLoadingSettings + Environment.NewLine + Translation.CloseAnyway;
                var message = new MessageInteraction(text, title,MessageOptions.YesCancel, MessageIcon.Warning);
                _interactionRequest.Raise(message, ResolveInteractionResult);
            }
            else
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
        }

        private void ResolveInteractionResult(MessageInteraction interactionResult)
        {
            if (interactionResult.Response == MessageResponse.Yes)
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Success));
            else
                IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(ResponseStatus.Cancel));
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
