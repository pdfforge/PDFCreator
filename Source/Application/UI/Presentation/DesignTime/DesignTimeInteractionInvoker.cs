using pdfforge.Obsidian;
using pdfforge.Obsidian.Interaction;
using pdfforge.Obsidian.Interaction.DialogInteractions;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeInteractionInvoker : IInteractionInvoker
    {
        public void Invoke<T>(T interaction) where T : IInteraction
        {
        }

        public void InvokeNonBlocking<T>(T interaction, Action<T> callback) where T : IInteraction
        {
        }

        public void InvokeNonBlocking<T>(T interaction) where T : IInteraction
        {
        }

        public void Invoke(SaveFileInteraction interaction)
        {
        }

        public void Invoke(FolderBrowserInteraction interaction)
        {
        }

        public void Invoke(ColorInteraction interaction)
        {
        }

        public void Invoke(FontInteraction interaction)
        {
        }

        public void Invoke(OpenFileInteraction interaction)
        {
        }
    }
}
