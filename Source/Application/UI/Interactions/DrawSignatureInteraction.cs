using pdfforge.Obsidian.Interaction;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class DrawSignatureInteraction : IInteraction
    {
        public string Title { get; }
        public bool Success { get; set; }
        public string SignatureFilePath { get; set; }

        public DrawSignatureInteraction(string title)
        {
            Title = title;
        }
    }
}
