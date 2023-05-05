using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.Core.Controller;
using pdfforge.PDFCreator.Core.DirectConversion;
using Prism.Commands;

namespace pdfforge.PDFCreator.UI.Presentation.Commands
{
    public class SelectFileViaDialogAndConvertCommand : DelegateCommandBase
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IFileConversionAssistant _fileConversionAssistant;

        public SelectFileViaDialogAndConvertCommand(IInteractionInvoker interactionInvoker, IFileConversionAssistant fileConversionAssistant)
        {
            _interactionInvoker = interactionInvoker;
            _fileConversionAssistant = fileConversionAssistant;
        }

        protected override void Execute(object parameter)
        {
            var interaction = new OpenFileInteraction();
            interaction.Multiselect = true;

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success)
                return;

            _fileConversionAssistant.HandleFileListWithoutTooManyFilesWarning(interaction.FileNames, new AppStartParameters());
        }

        protected override bool CanExecute(object parameter)
        {
            return true;
        }
    }
}
