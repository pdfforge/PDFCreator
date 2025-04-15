using System.Windows.Forms;
using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.Utilities.Messages;

namespace pdfforge.PDFCreator.UI.Presentation.Messages
{
    public class MessageHelper(IInteractionInvoker interactionInvoker) : IMessageHelper
    {
        public MessageResponse ShowMessage(string message, string title, MessageOptions options, MessageIcon icon, MessageResponse happyPathResponse = MessageResponse.Cancel)
        {
            var interaction = new MessageInteraction(message, title, options, icon);
            interactionInvoker.Invoke(interaction);
            return interaction.Response;
        }

        public void ShowHelp(string helpFile, string topic)
        {
            System.Windows.Forms.Help.ShowHelp(null, helpFile, HelpNavigator.Topic, topic);
        }
    }
}
