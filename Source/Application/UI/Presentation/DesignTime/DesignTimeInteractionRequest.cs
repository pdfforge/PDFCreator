using pdfforge.Obsidian.Interaction;
using pdfforge.Obsidian.Trigger;
using System;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeInteractionRequest : IInteractionRequest
    {
        public void Raise<T>(T context) where T : IInteraction
        { }

        public void Raise<T>(T context, Action<T> callback) where T : IInteraction
        { }

        public Task<T> RaiseAsync<T>(T context) where T : IInteraction
        {
            throw new NotImplementedException();
        }

        public event EventHandler<InteractionRequestEventArgs> Raised;
    }
}
