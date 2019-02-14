using pdfforge.Obsidian.Interaction;
using System;

namespace pdfforge.PDFCreator.UI.Interactions
{
    public class UpdateDownloadInteraction : IInteraction
    {
        public readonly Action StartDownloadCallback;

        public UpdateDownloadInteraction(Action startDownloadCallback)
        {
            StartDownloadCallback = startDownloadCallback;
        }

        public bool Success { get; set; }
    }
}
