using Optional;
using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace pdfforge.PDFCreator.UI.Presentation.Helper
{
    public interface IOpenFileInteractionHelper
    {
        Option<string> StartOpenFileInteraction(string currentPath, string title, string filter);
        Option<List<string>> StartOpenMultipleFilesInteraction(string currentPath, string title, string filter);
    }

    public class OpenFileInteractionHelper : IOpenFileInteractionHelper
    {
        private readonly IInteractionInvoker _interactionInvoker;

        public OpenFileInteractionHelper(IInteractionInvoker interactionInvoker)
        {
            _interactionInvoker = interactionInvoker;
        }

        public Option<string> StartOpenFileInteraction(string currentPath, string title, string filter)
        {
            var interaction = new OpenFileInteraction();
            interaction.Title = title;
            interaction.Filter = filter;

            if (!string.IsNullOrWhiteSpace(currentPath))
            {
                try
                {
                    interaction.FileName = Path.GetFileName(currentPath);
                    interaction.InitialDirectory = Path.GetDirectoryName(currentPath);
                }
                catch (ArgumentException)
                {
                }
            }

            _interactionInvoker.Invoke(interaction);

            return interaction.Success ? Option.Some(interaction.FileName) : Option.None<string>();
        }

        public Option<List<string>> StartOpenMultipleFilesInteraction(string currentPath, string title, string filter)
        {
            var interaction = new OpenFileInteraction();
            interaction.Title = title;
            interaction.Filter = filter;
            interaction.Multiselect = true;

            if (!string.IsNullOrWhiteSpace(currentPath))
            {
                try
                {
                    interaction.FileName = Path.GetFileName(currentPath);
                    interaction.InitialDirectory = Path.GetDirectoryName(currentPath);
                }
                catch (ArgumentException)
                {
                }
            }

            _interactionInvoker.Invoke(interaction);

            return interaction.Success ? Option.Some(interaction.FileNames.ToList()) : Option.None<List<string>>();
        }
    }
}
