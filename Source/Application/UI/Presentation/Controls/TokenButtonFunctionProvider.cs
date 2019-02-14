using Optional;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.Controls
{
    public interface ITokenButtonFunctionProvider
    {
        Func<string, Option<string>> GetBrowseFolderFunction(string description);

        Func<string, Option<string>> GetBrowseFileFunction(string title, string filter);
    }

    public class TokenButtonFunctionProvider : ITokenButtonFunctionProvider
    {
        private readonly IInteractionInvoker _interactionInvoker;
        private readonly IOpenFileInteractionHelper _openFileInteractionHelper;

        public TokenButtonFunctionProvider(IInteractionInvoker interactionInvoker, IOpenFileInteractionHelper openFileInteractionHelper)
        {
            _interactionInvoker = interactionInvoker;
            _openFileInteractionHelper = openFileInteractionHelper;
        }

        public Func<string, Option<string>> GetBrowseFolderFunction(string description)
        {
            return s => ExecuteFolderBrowse(description, s);
        }

        private Option<string> ExecuteFolderBrowse(string description, string initalText)
        {
            var interaction = new FolderBrowserInteraction
            {
                Description = description,
                ShowNewFolderButton = true,
                SelectedPath = initalText
            };

            _interactionInvoker.Invoke(interaction);

            if (!interaction.Success || string.IsNullOrWhiteSpace(interaction.SelectedPath))
                return Option.None<string>();

            return interaction.SelectedPath.Some();
        }

        public Func<string, Option<string>> GetBrowseFileFunction(string title, string filter)
        {
            return s => _openFileInteractionHelper.StartOpenFileInteraction(s, title, filter);
        }
    }
}
