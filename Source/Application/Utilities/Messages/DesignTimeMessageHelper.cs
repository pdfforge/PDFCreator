namespace pdfforge.PDFCreator.Utilities.Messages
{
    public class DesignTimeMessageHelper: IMessageHelper
    {
        public MessageResponse ShowMessage(string message, string title, MessageOptions options, MessageIcon icon, MessageResponse happyPathResponse = MessageResponse.Cancel)
        {
            return happyPathResponse;
        }

        public void ShowHelp(string helpFile, string topic)
        {
            
        }
    }
}
