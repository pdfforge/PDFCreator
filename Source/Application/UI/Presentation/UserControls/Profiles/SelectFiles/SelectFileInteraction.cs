using pdfforge.Obsidian.Interaction;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.UserControls.Profiles.SelectFiles
{
    public enum SelectFileInteractionResult
    {
        Apply,
        Remove,
        Cancel
    }

    public class SelectFileInteraction : IInteraction
    {
        public string Title { get; }
        public string File { get; set; }
        public bool ShowRemoveButton { get; }
        public List<string> Tokens { get; }
        public string Filter { get; }
        public SelectFileInteractionResult Result { get; set; } = SelectFileInteractionResult.Cancel;

        public SelectFileInteraction(string title, string initialFile, bool showRemoveButton, List<string> tokens, string filter)
        {
            Title = title;
            File = initialFile;
            ShowRemoveButton = showRemoveButton;
            Tokens = tokens;
            Filter = filter;
        }
    }
}
