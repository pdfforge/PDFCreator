using pdfforge.Obsidian.Trigger;
using pdfforge.PDFCreator.UI.Interactions;
using System;
using System.Windows.Input;

namespace pdfforge.PDFCreator.UI.Presentation.Commands.IniCommands
{
    public class LoadSpecificProfilesCommand : ICommand
    {
        private readonly IInteractionRequest _interactionRequest;

        public LoadSpecificProfilesCommand(IInteractionRequest interactionRequest)
        {
            _interactionRequest = interactionRequest;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var interaction = new LoadSpecificProfileInteraction();
            _interactionRequest.Raise(interaction);
        }

#pragma warning disable 67

        public event EventHandler CanExecuteChanged;

#pragma warning restore 67
    }
}
