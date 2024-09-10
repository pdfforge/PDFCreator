using System;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.Core.Services;
using pdfforge.PDFCreator.Core.Services.Macros;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Feedback;
using Prism.Commands;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class FeedbackCommand : DelegateCommandBase, IWaitableCommand
    {
        private readonly IInteractionInvoker _interactionInvoker;

        public FeedbackCommand(IInteractionInvoker interactionInvoker)
        {
            _interactionInvoker = interactionInvoker;
        }

        protected override void Execute(object parameter)
        {
            var interaction = new FeedbackInteraction();
            _interactionInvoker.Invoke(interaction);
        }

        protected override bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler<MacroCommandIsDoneEventArgs> IsDone;
    }
}
