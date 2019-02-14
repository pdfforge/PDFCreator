using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.UI.Presentation.NavigationChecks;
using pdfforge.PDFCreator.UI.Presentation.ViewModelBases;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.EvaluateSettingsCommands
{
    public abstract class EvaluateSettingsAndNotifyUserCommandBase : TranslatableCommandBase<EvaluateSettingsAndNotifyUserTranslation>, IWaitableCommand
    {
        private readonly IInteractionRequest _interactionRequest;

        public EvaluateSettingsAndNotifyUserCommandBase(IInteractionRequest interactionRequest,
            ITranslationUpdater translationUpdater)
            : base(translationUpdater)
        {
            _interactionRequest = interactionRequest;
        }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object obj)
        {
            var result = EvaluateRelevantSettings();
            var interaction = DetermineInteraction(result);

            if (interaction != null)
                _interactionRequest.Raise(interaction, ResolveInteractionResult);
            else
                RaiseIsDone(ResponseStatus.Success);
        }

        protected abstract void ResolveInteractionResult(MessageInteraction interactionResult);

        protected abstract SettingsCheckResult EvaluateRelevantSettings();

        protected abstract MessageInteraction DetermineInteraction(SettingsCheckResult result);

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;

        protected void RaiseIsDone(ResponseStatus responseStatus)
        {
            IsDone?.Invoke(this, new MacroCommandIsDoneEventArgs(responseStatus));
        }
    }
}
